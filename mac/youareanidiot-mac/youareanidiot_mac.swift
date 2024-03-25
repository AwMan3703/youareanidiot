//
//  youareanidiot_mac.swift
//  youareanidiot-mac
//

import SwiftUI

@main
struct youareanidiot_mac: App {
    @Environment(\.openWindow) private var openWindow
    
    public static let sherk = Bundle.main.url(forResource: "sherk", withExtension: "mov")!
    
    var body: some Scene {
        WindowGroup(id: "main-window") {
            ContentView(videoURL: youareanidiot_mac.sherk)
                .disabled(true)
                .onReceive(NotificationCenter.default.publisher(for: NSWindow.willCloseNotification)) {_ in
                    openWindow(id: "main-window")
                    openWindow(id: "main-window")
                }
        }
    }
}
