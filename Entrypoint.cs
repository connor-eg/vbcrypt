using System;

public class Entrypoint
{
	public Entrypoint()
	{
		static void Main(string[] args)
		{
			for (string arg : args)
			{
				Console.WriteLine(arg);
			}
		}
	}
}
