//
//  ContentView.swift
//  youareanidiot-mac
//

import SwiftUI
import AVKit

struct ContentView: View {
    private var player: AVQueuePlayer
    private var playerLooper: AVPlayerLooper
    
    var body: some View {
        VideoPlayer(player: player)
            .onAppear { player.play() }
            .onDisappear{ player.pause() }
            .controlSize(ControlSize.small)
            .allowsHitTesting(false)
    }
    
    init(videoURL: URL) {
        let asset = AVAsset(url: videoURL)
        let item = AVPlayerItem(asset: asset)
        
        player = AVQueuePlayer(playerItem: item)
        playerLooper = AVPlayerLooper(player: player, templateItem: item)
    }
}

#Preview {
    ContentView(videoURL: youareanidiot_mac.sherk)
        .disabled(true)
}
