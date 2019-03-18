/*
 * Created by SharpDevelop.
 * User: zeybey1
 * Date: 11/4/2017
 * Time: 3:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SDL2;

namespace sdlTest
{
	/// Description of CardGame.
	public class CardGame
	{
		
		bool favorite = false;
		int gameId;
		string name;
		IntPtr nameSurface;
		
		
		public CardGame(string newName, int newId)
		{
			name = newName;
			gameId = newId;
		}
		
		public void generateNameSurface(IntPtr font, SDL.SDL_Color color)
		{
			nameSurface = SDL_ttf.TTF_RenderText_Solid(font, name,  color);
		}
		
		public void toggleFavorite()
		{
			favorite = !favorite;
		}
		
		
		
		
		public IntPtr getNameSurface()
		{
			return nameSurface;
		}
		
		public bool isFavorite()
		{
			return favorite;
		}
		
		public int getId()
		{
			return gameId;
		}
		
		public string getName()
		{
			return name;
		}
	}
}
