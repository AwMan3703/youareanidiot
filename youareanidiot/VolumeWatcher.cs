using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;

namespace youareanidiot;

public class VolumeWatcher : IDisposable
{
    private readonly MMDeviceEnumerator _enumerator = new();
    private readonly MMDevice _device;
    private AudioSessionControl? _session;
    private SessionEventsHandler? _sessionHandler;
    private float _lastMasterVolume;
    private bool _lastMasterMute;
    private float _lastAppVolume;
    private bool _lastAppMute;

    public event Action<float>? MasterVolumeChange;
    public event Action<bool>? MasterMuteChange;
    public event Action<float>? AppVolumeChange;
    public event Action<bool>? AppMuteChange; 

    public VolumeWatcher()
    {
        _device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        _lastMasterVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar;
        _lastMasterMute = _device.AudioEndpointVolume.Mute;
        
        _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        _device.AudioSessionManager.OnSessionCreated += AudioSessionManager_OnSessionCreated;
        AttachToOwnSessionIfNotYetFound();
    }

    private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
    {
        if (Math.Abs(data.MasterVolume - _lastMasterVolume) > 0.01)
            MasterVolumeChange?.Invoke(data.MasterVolume);
        if (data.Muted != _lastMasterMute)
            MasterMuteChange?.Invoke(data.Muted);

        _lastMasterVolume = data.MasterVolume;
        _lastMasterMute = data.Muted;
    }
    
    private void AudioSessionManager_OnSessionCreated(object? sender, IAudioSessionControl newSession)
    {
        try
        {
            var session = new AudioSessionControl(newSession);

            uint myPid = (uint)Process.GetCurrentProcess().Id;
            if (session.GetProcessID != myPid)
                return;

            AttachToOwnSessionIfNotYetFound(session);
        }
        catch { /**/ }
    }
    
    private void AttachToOwnSessionIfNotYetFound(AudioSessionControl? alreadyFoundSession = null)
    {
        if (_session != null || _sessionHandler != null) return;

        if (alreadyFoundSession != null)
        {
            _session = alreadyFoundSession;
        }
        else
        {
            uint myPid = (uint)Process.GetCurrentProcess().Id;
            var sessions = _device.AudioSessionManager.Sessions;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session.GetProcessID != myPid)
                    continue;

                _session = session;
                break;
            }
        }

        if (_session == null) return;

        _lastAppVolume = _session.SimpleAudioVolume.Volume;
        _lastAppMute = _session.SimpleAudioVolume.Mute;

        _sessionHandler = new SessionEventsHandler(this);
        _session.RegisterEventClient(_sessionHandler);
        
        Console.WriteLine("Found audio session, now monitoring per-app volume in addition to master volume.");
    }

    public void SetMasterVolume(float volume)
    {
        _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
    }

    public void SetAppVolume(float volume)
    {
        if (_session == null) return;
        _session.SimpleAudioVolume.Volume = volume;
    }

    public void SetMasterMute(bool mute)
    {
        _device.AudioEndpointVolume.Mute = mute;
    }

    public void SetAppMute(bool mute)
    {
        if (_session == null) return;
        _session.SimpleAudioVolume.Mute = mute;
    }

    public void Dispose()
    {
        _device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
        _device.AudioSessionManager.OnSessionCreated -= AudioSessionManager_OnSessionCreated;

        _device?.Dispose();
        _enumerator?.Dispose();
        
        if (_session != null && _sessionHandler != null)
            _session.UnRegisterEventClient(_sessionHandler);
    }
    
    private sealed class SessionEventsHandler(VolumeWatcher owner) : IAudioSessionEventsHandler
    {
        public void OnVolumeChanged(float volume, bool isMuted)
        {
            if (Math.Abs(volume - owner._lastAppVolume) > 0.01f)
                owner.AppVolumeChange?.Invoke(volume);

            if (isMuted != owner._lastAppMute)
                owner.AppMuteChange?.Invoke(isMuted);

            owner._lastAppVolume = volume;
            owner._lastAppMute = isMuted;
        }

        // Don't need these but they have to be implemented
        public void OnDisplayNameChanged(string displayName) { }
        public void OnIconPathChanged(string iconPath) { }
        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex) { }
        public void OnGroupingParamChanged(ref Guid groupingId) { }
        public void OnStateChanged(AudioSessionState state) { }
        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason) { }
    }
}