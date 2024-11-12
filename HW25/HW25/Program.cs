NetworkCommunicator communicator = new NetworkCommunicator();

// HTTP
string httpGetResponse = await communicator.SendHttpGetAsync("https://jsonplaceholder.typicode.com/posts/1");
string httpPostResponse = await communicator.SendHttpPostAsync("https://jsonplaceholder.typicode.com/posts", "{ \"title\": \"foo\", \"body\": \"bar\", \"userId\": 1 }");

// UDP
communicator.SendUdpMessage("127.0.0.1", 5001, "Hello UDP!");
string udpResponse = await communicator.ReceiveUdpMessageAsync(5001);

// TCP
await communicator.SendTcpMessageAsync("127.0.0.1", 5000, "Hello TCP!");
string tcpResponse = await communicator.ReceiveTcpMessageAsync(5000);




