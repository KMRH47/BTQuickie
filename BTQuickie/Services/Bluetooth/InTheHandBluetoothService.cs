using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTQuickie.Models.Device;
using BTQuickie.Models.Settings;
using BTQuickie.Services.Settings;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using BluetoothDeviceInfo = InTheHand.Net.Sockets.BluetoothDeviceInfo;

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

  public async Task<IReadOnlyCollection<BluetoothDevice>> DiscoverDevices() {
    TimeSpan inquiryLength = TimeSpan.FromMilliseconds(userSettings.DiscoveryInfo.DiscoveryTimeMs);
    bluetoothClient.InquiryLength = inquiryLength;

    IReadOnlyCollection<BluetoothDeviceInfo>? devices = bluetoothClient.DiscoverDevices();
    List<BluetoothDevice> mappedDevices = devices.Select(MapModel).ToList();

    return await Task.FromResult<IReadOnlyCollection<BluetoothDevice>>(mappedDevices);
  }

  public IReadOnlyCollection<BluetoothDevice> GetPairedDevices() =>
    bluetoothClient.PairedDevices.Select(MapModel).ToList();

  public async Task ConnectAsync(BluetoothDevice bluetoothDeviceInfo) {
    if (bluetoothClient.Connected &&
        bluetoothClient.Client.RemoteEndPoint?.ToString()?.Split(':')[0] == bluetoothDeviceInfo.Address) {
      await bluetoothClient.Client.DisconnectAsync(true);
      bluetoothClient.Close();
      bluetoothClient = CreateClient();
      return;
    }

    // Manually handle exception propagation since they are consumed inside Task.Run
    Task task = Task.Run(() => {
      try {
        BluetoothDeviceInfo? bluetoothDevice =
          bluetoothClient.PairedDevices.First(device => device.DeviceAddress.ToString() == bluetoothDeviceInfo.Address);
        Guid? serviceGuid = bluetoothDevice.InstalledServices?.FirstOrDefault() ?? BluetoothService.SerialPort;
        bluetoothClient.Connect(BluetoothAddress.Parse(bluetoothDeviceInfo.Address), serviceGuid.Value);
        return Task.CompletedTask;
      }
      catch (Exception e) {
        return Task.FromException(e);
      }
    });

    await task;

    if (task.IsFaulted) {
      throw task.Exception!;
    }
  }

  public void PairRequest(BluetoothDevice bluetoothDeviceInfo, string? pin = null) =>
    BluetoothSecurity.PairRequest(BluetoothAddress.Parse(bluetoothDeviceInfo.Address), pin);

  public void Disconnect() {
    bluetoothClient.Dispose();
    bluetoothClient = CreateClient();
  }

  private BluetoothClient CreateClient() {
    float inquiryLengthMs = userSettings.DiscoveryInfo.DiscoveryTimeMs;
    return new BluetoothClient { InquiryLength = TimeSpan.FromMilliseconds(inquiryLengthMs) };
  }

  private static BluetoothDevice MapModel(BluetoothDeviceInfo device) =>
    new(
      Name: device.DeviceName,
      Address: device.DeviceAddress.ToString(),
      State: GetState(device)
    );

  private static BluetoothConnectionState GetState(BluetoothDeviceInfo device) =>
    device switch {
      { Connected: true } => BluetoothConnectionState.Connected,
      { Authenticated: true } => BluetoothConnectionState.Paired,
      _ => BluetoothConnectionState.Discovered
    };
}