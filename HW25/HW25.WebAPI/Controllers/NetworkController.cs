using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace HW25.WebAPI.Controllers
{
	public class NetworkController : Controller
	{
		private readonly HttpClient _httpClient = new();

		[HttpPost("tcp")]
		public async Task<IActionResult> SendTcpMessageAsync(string serverIp, int port, string message)
		{
			using TcpClient client = new TcpClient();
			await client.ConnectAsync(serverIp, port);

			NetworkStream stream = client.GetStream();
			byte[] data = Encoding.UTF8.GetBytes(message);

			await stream.WriteAsync(data, 0, data.Length);
			return Ok("TCP Message Sent: " + message);
		}

		[HttpGet("tcp")]
		public async Task<ActionResult<string>> ReceiveTcpMessageAsync(int port)
		{
			using TcpListener listener = new TcpListener(IPAddress.Any, port);
			listener.Start();

			using TcpClient client = await listener.AcceptTcpClientAsync();
			NetworkStream stream = client.GetStream();

			byte[] buffer = new byte[1024];
			int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

			string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			return Ok("TCP Message Received: " + message);
		}

		[HttpPost("udp")]
		public IActionResult SendUdpMessage(string serverIp, int port, string message)
		{
			using UdpClient udpClient = new UdpClient();
			byte[] data = Encoding.UTF8.GetBytes(message);

			udpClient.Send(data, data.Length, serverIp, port);
			return Ok("UDP Message Sent: " + message);
		}

		[HttpGet("udp")]
		public async Task<ActionResult<string>> ReceiveUdpMessageAsync(int port)
		{
			using UdpClient udpClient = new UdpClient(port);
			Console.WriteLine("UDP Listener started, waiting for messages...");

			UdpReceiveResult result = await udpClient.ReceiveAsync();
			string message = Encoding.UTF8.GetString(result.Buffer);
			return Ok("UDP Message Received: " + message);
		}


		[HttpPost("http")]
		public async Task<ActionResult<string>> SendHttpPostAsync(string url, string content)
		{
			HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
			response.EnsureSuccessStatusCode();

			string responseBody = await response.Content.ReadAsStringAsync();
			return Ok("HTTP POST Response: " + responseBody);
		}

		[HttpGet("http")]
		public async Task<ActionResult<string>> SendHttpGetAsync(string url)
		{
			HttpResponseMessage response = await _httpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();

			string responseBody = await response.Content.ReadAsStringAsync();
			return Ok("HTTP GET Response: " + responseBody);
		}	
	}
}
