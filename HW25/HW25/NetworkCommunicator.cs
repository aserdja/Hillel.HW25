using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NetworkCommunicator
{
	private readonly HttpClient _httpClient = new();

	public async Task SendTcpMessageAsync(string serverIp, int port, string message)
	{
		using TcpClient client = new TcpClient();
		await client.ConnectAsync(serverIp, port);

		NetworkStream stream = client.GetStream();
		byte[] data = Encoding.UTF8.GetBytes(message);

		await stream.WriteAsync(data, 0, data.Length);
		Console.WriteLine("TCP Message Sent: " + message);
	}

	public async Task<string> ReceiveTcpMessageAsync(int port)
	{
		using TcpListener listener = new TcpListener(IPAddress.Any, port);
		listener.Start();
		Console.WriteLine("TCP Listener started, waiting for connection...");

		using TcpClient client = await listener.AcceptTcpClientAsync();
		NetworkStream stream = client.GetStream();

		byte[] buffer = new byte[1024];
		int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

		string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
		Console.WriteLine("TCP Message Received: " + message);

		return message;
	}

	public void SendUdpMessage(string serverIp, int port, string message)
	{
		using UdpClient udpClient = new UdpClient();
		byte[] data = Encoding.UTF8.GetBytes(message);

		udpClient.Send(data, data.Length, serverIp, port);
		Console.WriteLine("UDP Message Sent: " + message);
	}

	public async Task<string> ReceiveUdpMessageAsync(int port)
	{
		using UdpClient udpClient = new UdpClient(port);
		Console.WriteLine("UDP Listener started, waiting for messages...");

		UdpReceiveResult result = await udpClient.ReceiveAsync();
		string message = Encoding.UTF8.GetString(result.Buffer);
		Console.WriteLine("UDP Message Received: " + message);

		return message;
	}

	public async Task<string> SendHttpGetAsync(string url)
	{
		HttpResponseMessage response = await _httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		Console.WriteLine("HTTP GET Response: " + responseBody);

		return responseBody;
	}

	public async Task<string> SendHttpPostAsync(string url, string content)
	{
		HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
		HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		Console.WriteLine("HTTP POST Response: " + responseBody);

		return responseBody;
	}
}
