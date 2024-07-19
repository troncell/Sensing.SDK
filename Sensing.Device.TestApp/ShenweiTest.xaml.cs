using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.AspNetCore.SignalR.Client;
using Sensing.Device.TestApp.Input;

namespace Sensing.Device.TestApp;

public partial class ShenweiTest : Window
{
    Microsoft.AspNetCore.SignalR.Client.HubConnection connection;
    Microsoft.AspNetCore.SignalR.Client.HubConnection connection1;
    Microsoft.AspNetCore.SignalR.Client.HubConnection connection2;
    Microsoft.AspNetCore.SignalR.Client.HubConnection connection3;
    Microsoft.AspNetCore.SignalR.Client.HubConnection connection4;
    public ShenweiTest()
    {
        InitializeComponent();
        connection = new HubConnectionBuilder()
            .WithUrl("http://192.168.3.65:8080/LocalSensingDevice")
            .Build();
        connection1 = new HubConnectionBuilder()
            .WithUrl("http://192.168.3.65:8080/LocalSensingDevice")
            .Build();
        connection2 = new HubConnectionBuilder()
            .WithUrl("http://192.168.3.65:8080/LocalSensingDevice")
            .Build();
        connection3 = new HubConnectionBuilder()
            .WithUrl("http://192.168.3.65:8080/LocalSensingDevice")
            .Build();
        connection4 = new HubConnectionBuilder()
            .WithUrl("http://192.168.3.65:8080/LocalSensingDevice")
            .Build();
        
        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };
        
        connection1.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection1.StartAsync();
        };
        connection2.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection1.StartAsync();
        };
        connection3.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection1.StartAsync();
        };
        connection4.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection1.StartAsync();
        };
    }

    private async void connectButton_Click(object sender, RoutedEventArgs e)
    {
        connection.On<object>("received",  message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList.Items.Add(message.ToString());
            });
        });

        try
        {
            await connection.StartAsync();
            messagesList.Items.Add("Connection started");
            connectButton.IsEnabled = false;
            sendButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click1(object sender, RoutedEventArgs e)
    {
        connection1.On<object>("received",  message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList1.Items.Add(message.ToString());
            });
        });

        try
        {
            await connection1.StartAsync();
            messagesList1.Items.Add("Connection started");
            connectButton1.IsEnabled = false;
            sendButton1.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList1.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click2(object sender, RoutedEventArgs e)
    {
        connection2.On<object>("received",  message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList2.Items.Add(message.ToString());
            });
        });

        try
        {
            await connection2.StartAsync();
            messagesList2.Items.Add("Connection started");
            connectButton2.IsEnabled = false;
            sendButton2.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList2.Items.Add(ex.Message);
        }
    }
    
    private async void connectButton_Click3(object sender, RoutedEventArgs e)
    {
        connection3.On<object>("received",  message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList3.Items.Add(message.ToString());
            });
        });

        try
        {
            await connection3.StartAsync();
            messagesList3.Items.Add("Connection started");
            connectButton3.IsEnabled = false;
            sendButton3.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList3.Items.Add(ex.Message);
        }
    }

    private async void connectButton_Click4(object sender, RoutedEventArgs e)
    {
        connection4.On<object>("received",  message =>
        {
            this.Dispatcher.Invoke(() =>
            {
                receivedList4.Items.Add(message.ToString());
            });
        });

        try
        {
            await connection4.StartAsync();
            messagesList4.Items.Add("Connection started");
            connectButton4.IsEnabled = false;
            sendButton4.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messagesList4.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click1(object sender, RoutedEventArgs e)
    {
        try
        {
            var deviceLoginInput = new DeviceLoginInput();
            DeviceAuth deviceAuth = new DeviceAuth();
            deviceAuth.DeviceId = long.Parse(DeviceIdTB1.Text);
            deviceAuth.SubKey = SubKeyTB1.Text;
            deviceLoginInput.CommonText = deviceAuth;
            deviceLoginInput.Type = "Auth";
            await connection1.InvokeAsync("NotifyDataChanged",
                deviceLoginInput, 1);
        }
        catch (Exception ex)
        {
            messagesList1.Items.Add(ex.Message);
        }
    }
    
    
    private async void loginButton_Click2(object sender, RoutedEventArgs e)
    {
        try
        {
            var deviceLoginInput = new DeviceLoginInput();
            DeviceAuth deviceAuth = new DeviceAuth();
            deviceAuth.DeviceId = long.Parse(IDevice1.Text);
            deviceAuth.SubKey = SDevice1.Text;
            deviceLoginInput.CommonText = deviceAuth;
            deviceLoginInput.Type = "Auth";
            await connection2.InvokeAsync("NotifyDataChanged",
                deviceLoginInput, 2);
        }
        catch (Exception ex)
        {
            messagesList2.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click3(object sender, RoutedEventArgs e)
    {
        try
        {
            var deviceLoginInput = new DeviceLoginInput();
            DeviceAuth deviceAuth = new DeviceAuth();
            deviceAuth.DeviceId = long.Parse(IDevice2.Text);
            deviceAuth.SubKey = SDevice2.Text;
            deviceLoginInput.CommonText = deviceAuth;
            deviceLoginInput.Type = "Auth";
            await connection3.InvokeAsync("NotifyDataChanged",
                deviceLoginInput, 2);
        }
        catch (Exception ex)
        {
            messagesList3.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click4(object sender, RoutedEventArgs e)
    {
        try
        {
            var deviceLoginInput = new DeviceLoginInput();
            DeviceAuth deviceAuth = new DeviceAuth();
            deviceAuth.DeviceId = long.Parse(IDevice3.Text);
            deviceAuth.SubKey = SDevice3.Text;
            deviceLoginInput.CommonText = deviceAuth;
            deviceLoginInput.Type = "Auth";
            await connection4.InvokeAsync("NotifyDataChanged",
                deviceLoginInput, 2);
        }
        catch (Exception ex)
        {
            messagesList4.Items.Add(ex.Message);
        }
    }
    
    private async void loginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var deviceLoginInput = new DeviceLoginInput();
            DeviceAuth deviceAuth = new DeviceAuth();
            deviceAuth.DeviceId = long.Parse(DeviceIdTB.Text);
            deviceAuth.SubKey = SubKeyTB.Text;
            deviceLoginInput.CommonText = deviceAuth;
            deviceLoginInput.Type = "Auth";
            await connection.InvokeAsync("NotifyDataChanged",
                deviceLoginInput, 1);
        }
        catch (Exception ex)
        {
            messagesList.Items.Add(ex.Message);
        }
    }

    private void AreaID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }

    private async void lockButton_Click(object sender, RoutedEventArgs e)
    {
        AreaLockInput input = new AreaLockInput();
        input.Type = "AreaLock";
        AreaLock _lock = new AreaLock();
        _lock.HallAreaId = long.Parse(LockAreaID.Text);
        input.CommonText = _lock;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }
    
    private async void lockButton_Click1(object sender, RoutedEventArgs e)
    {
        AreaLockInput input = new AreaLockInput();
        input.Type = "AreaLock";
        AreaLock _lock = new AreaLock();
        _lock.HallAreaId = long.Parse(LockAreaID1.Text);
        input.CommonText = _lock;
        await connection1.InvokeAsync("NotifyDataChanged",
            input, 1);
    }
    
    
    private async void releaseButton_Click(object sender, RoutedEventArgs e)
    {
        AreaLockInput input = new AreaLockInput();
        input.Type = "AreaRelease";
        AreaLock _lock = new AreaLock();
        _lock.HallAreaId = long.Parse(ReleaseAreaID.Text);
        input.CommonText = _lock;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }
    
    private async void releaseButton_Click1(object sender, RoutedEventArgs e)
    {
        AreaLockInput input = new AreaLockInput();
        input.Type = "AreaRelease";
        AreaLock _lock = new AreaLock();
        _lock.HallAreaId = long.Parse(ReleaseAreaID1.Text);
        input.CommonText = _lock;
        await connection1.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void areaControlButton_Click(object sender, RoutedEventArgs e)
    {
        AreaControlInput input = new AreaControlInput();
        input.Type = "AreaControl";
        AreaControl control = new AreaControl();
        control.HallAreaId = long.Parse(ControlAreaID.Text);
        control.ControlId = long.Parse(AControlID.Text);
        input.CommonText = control;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);

    }

    private async void deviceControlButton_Click(object sender, RoutedEventArgs e)
    {
        DeviceControlInput input = new DeviceControlInput();
        input.Type = "DeviceControl";
        DeviceControl control = new DeviceControl();
        control.DeviceId = long.Parse(ControlDeviceID.Text);
        control.ControlId = long.Parse(DControlID.Text);
        input.CommonText = control;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void areaControlButton_Click1(object sender, RoutedEventArgs e)
    {
        AreaControlInput input = new AreaControlInput();
        input.Type = "AreaControl";
        AreaControl control = new AreaControl();
        control.HallAreaId = long.Parse(ControlAreaID1.Text);
        control.ControlId = long.Parse(AControlID1.Text);
        input.CommonText = control;
        await connection1.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void deviceControlButton_Click1(object sender, RoutedEventArgs e)
    {
        DeviceControlInput input = new DeviceControlInput();
        input.Type = "DeviceControl";
        DeviceControl control = new DeviceControl();
        control.DeviceId = long.Parse(ControlDeviceID1.Text);
        control.ControlId = long.Parse(DControlID1.Text);
        input.CommonText = control;
        await connection1.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void lockDButton_Click(object sender, RoutedEventArgs e)
    {
        DeviceLockInput input = new DeviceLockInput();
        input.Type = "DeviceLock";
        DeviceLock loDeviceLock = new DeviceLock();
        loDeviceLock.DeviceId = long.Parse(LockDeviceID.Text);
        input.CommonText = loDeviceLock;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void releaseDButton_Click(object sender, RoutedEventArgs e)
    {
        DeviceLockInput input = new DeviceLockInput();
        input.Type = "DeviceRelease";
        DeviceLock loDeviceLock = new DeviceLock();
        loDeviceLock.DeviceId = long.Parse(LockDeviceID.Text);
        input.CommonText = loDeviceLock;
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }

    private async void lockDButton_Click1(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void releaseDButton_Click1(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void getDeviceButton_Click(object sender, RoutedEventArgs e)
    {
        DeviceLockInput input = new DeviceLockInput();
        input.Type = "DeviceStatus";
        await connection.InvokeAsync("NotifyDataChanged",
            input, 1);
    }
}