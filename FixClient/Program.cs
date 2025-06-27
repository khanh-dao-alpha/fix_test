using Epam.FixAntenna.Constants.Fixt11; 
using Epam.FixAntenna.NetCore.FixEngine;
using Epam.FixAntenna.NetCore.FixEngine.Session.Util;
using Epam.FixAntenna.NetCore.Message;

namespace Client
{
  public class SimpleNewsBroadcaster
  {
	public static void Main(string[] args)
	{
		var sessionList = new List<IFixSession>();
		
		var sessions = SessionParametersBuilder.BuildSessionParametersList("./fixclient.properties");
		
		foreach (var kvp in sessions)
		{
			var sessionId = kvp.Key;
			var details = kvp.Value;
		
			Console.WriteLine($"=== Session: {sessionId} ===");
		
			var session = details.CreateInitiatorSession();
			sessionList.Add(session);
			session.SetFixSessionListener(new MyFixSessionListener(session));
		
			session.Connect();
		}
		
		
		foreach (var session in sessionList)
		{
			FixMessage messageContent = new FixMessage();
			messageContent.AddTag(148, "Hello there");
			messageContent.AddTag(33, 3);
			messageContent.AddTag(58, "line1");
			messageContent.AddTag(58, "line2");
			messageContent.AddTag(58, "line3");
		
			session.SendMessage("B", messageContent);
		}
		
		Console.Read();
		foreach (var session in sessionList)
		{
			if (SessionState.IsNotDisconnected(session.SessionState))
			{
				session.Disconnect("Shutting down...");
			}
			session.Dispose();
		}

	}

	private class MyFixSessionListener(IFixSession session) : IFixSessionListener
	{
		public void OnNewMessage(FixMessage message)
	  {
		Console.WriteLine($"{session.Parameters.SessionId}: New application level message type: " + message.GetTagValueAsString(Tags.MsgType) + "received");
	  }

	  public void OnSessionStateChange(SessionState sessionState)
	  {
		Console.WriteLine(session.Parameters.SenderCompId + " <-> " + session.Parameters.TargetCompId + "---" +
		                  $"{session.Parameters.SessionId}: Session state changed:" + sessionState);
		Console.WriteLine();
		if (sessionState == SessionState.Disconnected)
		{
		  session.Dispose();
		}
	  }
	}
  }
}