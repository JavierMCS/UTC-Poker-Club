/*
 * Created by SharpDevelop.
 * User: zeybey1
 * Date: 11/4/2017
 * Time: 6:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SDL2;

namespace sdlTest
{
	public class Program
	{
		
		public static unsafe void Main(string[] args)
		{
			
			GameSelectionInterface g = new GameSelectionInterface();
			
			g.init();
			g.run();
			
				
			return;			 
		}
	}
}
