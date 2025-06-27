using Epam.FixAntenna.NetCore.Configuration;
using Epam.FixAntenna.NetCore.FixEngine;
using Epam.FixAntenna.NetCore.Message;

namespace Server
{
  public class SimpleServer : IFixServerListener
  {
	public static void Main(string[] args)
	{
		var configuration = new Config("./fixengine.properties");
	    FixServer server = new FixServer(configuration); 
	    server.SetListener(new SimpleServer());
	  
	    server.Start(); 

		Console.Read(); 
		server.Stop(); 
	}

	public void NewFixSession(IFixSession session)
	{
	  try
	  {
		session.SetFixSessionListener(new MyFixSessionListener(session)); 
		
		session.Connect();
		Console.WriteLine("New FIX session accepted: " +
		                  session.Parameters.SenderCompId + " <-> " +
		                  session.Parameters.TargetCompId);	  }
	  catch (IOException e)
	  {
		  Console.WriteLine(e.Message);
	  }
	}

	private class MyFixSessionListener(IFixSession session) : IFixSessionListener
	{
	  public void OnSessionStateChange(SessionState sessionState)
	  {
		Console.WriteLine("Session state: " + sessionState);

		if (sessionState == SessionState.Disconnected)
		{
		  session.Dispose();
		  Console.WriteLine("Your session has been disconnected. Press ENTER to exit the programm.");
		}
	  }

	  public void OnNewMessage(FixMessage message)
	  {
		Console.WriteLine("New message is accepted: " + message.ToString());
		var ack = new FixMessage();
		ack.AddTag(35, "8"); // MsgType = ExecutionReport
		ack.AddTag(150, "0"); // OrdStatus = New
		ack.AddTag(39, "0");  // OrdStatus = New
		ack.AddTag(37, "12345"); // OrderID
		ack.AddTag(17, "1");     // ExecID

		session.SendMessage("8", ack);
	  }
	}
  }
}