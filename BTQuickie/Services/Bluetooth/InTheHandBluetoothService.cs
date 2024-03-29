﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTQuickie.Models.Settings;
using BTQuickie.Services.Settings;
using BluetoothDeviceInfoLocal = BTQuickie.Models.Device.BluetoothDeviceInfo;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Bluetooth
{
    public class InTheHandBluetoothService : IBluetoothService
    {
        private readonly UserSettings userSettings;
        private BluetoothClient bluetoothClient;

        public InTheHandBluetoothService(IApplicationSettingsProvider applicationSettingsProvider)
        {
            this.userSettings = applicationSettingsProvider.UserSettings;
            this.bluetoothClient = CreateClient();
        }

        public bool Connected => this.bluetoothClient.Connected;

        public Guid GuidSerialPort()
        {
            return BluetoothService.SerialPort;
        }

        public IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices()
        {
            TimeSpan inquiryLength = TimeSpan.FromMilliseconds(this.userSettings.DiscoveryInfo.DiscoveryTimeMs);
            this.bluetoothClient.InquiryLength = inquiryLength;
            return MapModel(this.bluetoothClient.DiscoverDevices());
        }

        public IEnumerable<BluetoothDeviceInfoLocal> PairedDevices()
        {
            return MapModel(this.bluetoothClient.PairedDevices);
        }

        public void Connect(string address, Guid serviceGuid)
        {
            this.bluetoothClient.Connect(BluetoothAddress.Parse(address), serviceGuid);
        }

        public async Task ConnectAsync(string address, Guid serviceGuid)
        {
            await Task.Run(() => this.bluetoothClient.Connect(BluetoothAddress.Parse(address), serviceGuid));
        }

        public void PairRequest(string address, string pin)
        {
            BluetoothSecurity.PairRequest(BluetoothAddress.Parse(address), pin);
        }

        public void Disconnect()
        {
            this.bluetoothClient.Dispose();
            this.bluetoothClient = CreateClient();
        }

        private static IReadOnlyCollection<BluetoothDeviceInfoLocal> MapModel(IEnumerable<BluetoothDeviceInfo> devices)
        {
            return devices.Select(bluetoothDeviceInfo =>
                                      new BluetoothDeviceInfoLocal(
                                          bluetoothDeviceInfo.DeviceName,
                                          bluetoothDeviceInfo.DeviceAddress.ToString())).ToList();
        }

        private BluetoothClient CreateClient()
        {
            float inquiryLengthMs = this.userSettings.DiscoveryInfo.DiscoveryTimeMs;
            return new BluetoothClient {InquiryLength = TimeSpan.FromMilliseconds(inquiryLengthMs)};
        }
    }
}