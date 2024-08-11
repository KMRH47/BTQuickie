using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTQuickie.Models.Settings;
using BTQuickie.Services.Settings;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using BluetoothDeviceInfoLocal = BTQuickie.Models.Device.BluetoothDeviceInfo;

namespace BTQuickie.Services.Bluetooth;

public class InTheHandBluetoothService : IBluetoothService
{
  private readonly UserSettings userSettings;
  private BluetoothClient bluetoothClient;

  public InTheHandBluetoothService(IApplicationSettingsProvider applicationSettingsProvider) {
    userSettings = applicationSettingsProvider.UserSettings;
    bluetoothClient = CreateClient();
  }

  public bool Connected => bluetoothClient.Connected;

  public Guid GuidSerialPort() => BluetoothService.SerialPort;

  public IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices() {
    TimeSpan inquiryLength = TimeSpan.FromMilliseconds(userSettings.DiscoveryInfo.DiscoveryTimeMs);
    bluetoothClient.InquiryLength = inquiryLength;
    return MapModel(bluetoothClient.DiscoverDevices());
  }

  public IEnumerable<BluetoothDeviceInfoLocal> PairedDevices() => MapModel(bluetoothClient.PairedDevices);

  public void Connect(string address, Guid serviceGuid) =>
    bluetoothClient.Connect(BluetoothAddress.Parse(address), serviceGuid);

  public async Task ConnectAsync(string address, Guid serviceGuid) =>
    await Task.Run(() => bluetoothClient.Connect(BluetoothAddress.Parse(address), serviceGuid));

  public void PairRequest(string address, string pin) =>
    BluetoothSecurity.PairRequest(BluetoothAddress.Parse(address), pin);

  public void Disconnect() {
    bluetoothClient.Dispose();
    bluetoothClient = CreateClient();
  }

  private static IReadOnlyCollection<BluetoothDeviceInfoLocal> MapModel(IEnumerable<BluetoothDeviceInfo> devices) {
    return devices.Select(bluetoothDeviceInfo =>
      new BluetoothDeviceInfoLocal(
        Name: bluetoothDeviceInfo.DeviceName,
        Address: bluetoothDeviceInfo.DeviceAddress.ToString(),
        IsPaired: bluetoothDeviceInfo.Remembered,
        IsConnected: bluetoothDeviceInfo.Connected
      )).ToList();
  }

  private BluetoothClient CreateClient() {
    float inquiryLengthMs = userSettings.DiscoveryInfo.DiscoveryTimeMs;
    return new BluetoothClient { InquiryLength = TimeSpan.FromMilliseconds(inquiryLengthMs) };
  }
}