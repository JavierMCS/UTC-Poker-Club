/*
 * Created by SharpDevelop.
 * User: zeybey1
 * Date: 11/4/2017
 * Time: 6:15 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SDL2;
using System.Collections.Generic;

namespace sdlTest
{
	class GameSelectionInterface
	{
		
		IntPtr window;
		IntPtr windowSurface;
		
		
		IntPtr font;
		IntPtr arrowSprite;
		IntPtr instructionTextSurface;
		IntPtr titleSprite;
		SDL.SDL_Color textColor;
		SDL.SDL_Color favoritesColor;
		
		SDL.SDL_Event e;
		
		List<CardGame> gameList = new List<CardGame>();
		List<CardGame> favoriteGameList = new List<CardGame>();
		
		uint clearColor;
		SDL.SDL_Rect screenRect;
		
		int cursorPointingTo = 0;
		int cursorMaxValue = 3;
		bool showOnlyFavorites = false;
		
		public unsafe void init()
		{
			SDL.SDL_Init( SDL.SDL_INIT_VIDEO);
			SDL_ttf.TTF_Init();
			
			window = SDL.SDL_CreateWindow("Card Games", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 600, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
			windowSurface = SDL.SDL_GetWindowSurface(window);
			arrowSprite = SDL.SDL_LoadBMP("arrow.bmp");
			titleSprite = SDL.SDL_LoadBMP("title.bmp");
			
			SDL.SDL_Surface* windowSDLSurface = (SDL.SDL_Surface*)windowSurface;
			clearColor = SDL.SDL_MapRGB(windowSDLSurface->format, 0, 100, 0);
			
			textColor.r = 255; textColor.b = 255; textColor.g = 255; textColor.a = 255;
			favoritesColor.r = 255; favoritesColor.b = 0; favoritesColor.g = 255; favoritesColor.a = 255;
			
			font = SDL_ttf.TTF_OpenFont("times.ttf", 20); 
			
			string instructionText;
			instructionText = "ENTER to select game, F to favorite, SPACE to toggle showing favorites";
			instructionTextSurface = SDL_ttf.TTF_RenderText_Solid(font, instructionText, textColor);
			
			
			screenRect.x = 0; screenRect.y = 0;
			screenRect.w = 600; screenRect.h = 480;
			
			
			//initialize the card games
			gameList.Add(new CardGame("Three Card Poker", 1));
			gameList[0].generateNameSurface(font, textColor);
			gameList.Add(new CardGame("Omaha High Poker", 2));
			gameList[1].generateNameSurface(font, textColor);
			gameList.Add(new CardGame("Three Card Draw", 3));
			gameList[2].generateNameSurface(font, textColor);
			gameList.Add(new CardGame("Pai Gow Poker", 4));
			gameList[3].generateNameSurface(font, textColor);
		}
		
		
		
		
		public unsafe void run()
		{
			bool quit = false;
			
			while (quit == false)
			{
				int gameSelect = 0;
				
				while(SDL.SDL_PollEvent(out e) == 1)
				{
					//handle closing the window
					if (e.type == SDL.SDL_EventType.SDL_QUIT)
					{
						quit = true;
					}
					
					//handle key inputs
					else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
					{
						switch(e.key.keysym.sym)
						{
								//move cursor up
							case SDL.SDL_Keycode.SDLK_UP:
								if (cursorPointingTo > 0)
								{
									cursorPointingTo--;
								}
								break;
								
								//move cursor down
							case SDL.SDL_Keycode.SDLK_DOWN:
								if (cursorPointingTo < cursorMaxValue)
								{
									cursorPointingTo++;
								}
								break;
								
								//play the currently selected game
							case SDL.SDL_Keycode.SDLK_RETURN:
								gameSelect = gameList[cursorPointingTo].getId();
								break;
								
								//toggle a game as a favorite
							case SDL.SDL_Keycode.SDLK_f:
								if (showOnlyFavorites == false)
								{
									gameList[cursorPointingTo].toggleFavorite();
									
									if (gameList[cursorPointingTo].isFavorite() == false)
									{
										gameList[cursorPointingTo].generateNameSurface(font, textColor);
									}
									else
									{
										gameList[cursorPointingTo].generateNameSurface(font, favoritesColor);
									}
								}
								break;
								
								//show only favorites
							case SDL.SDL_Keycode.SDLK_SPACE:
								if (showOnlyFavorites == false) //start showing all favorites
								{
									showOnlyFavorites = true;
									cursorPointingTo = 0;
									
									favoriteGameList.Clear();
									
									for(int i = 0; i < gameList.Count; i++){
										if (gameList[i].isFavorite())
										{
											favoriteGameList.Add(gameList[i]);
										}
									}
									
									cursorMaxValue = favoriteGameList.Count - 1;
								}
								else  //stop showing all favorites
								{
									showOnlyFavorites = false;
									cursorMaxValue = gameList.Count - 1;
								}
								break;
								
						}
					}
				}
				
				//if the user pressed enter to start a game
				if (gameSelect != 0)
				{
					if (gameSelect == 1)
					{
						ThreeCardPokerGame tcp = new ThreeCardPokerGame(window);
						tcp.run();
					}
				}
				
				draw();
				SDL.SDL_Delay(30);
			}
		}
		
		
		
		public unsafe void draw()
		{
			//clear the screen a dark green color
			SDL.SDL_FillRect(windowSurface, ref screenRect, clearColor);
			
			drawSprite(titleSprite, 145, 25);
					
			//draw the cursor
			drawSprite(arrowSprite, 170, 100 + cursorPointingTo*32);
						
			//draw each game name
			int gameNameY = 100;
			if (showOnlyFavorites == false)
			{
				for(int i = 0; i < gameList.Count; i++)
				{
					drawSprite(gameList[i].getNameSurface(), 200, gameNameY);
					gameNameY += 32;
				}
			}
			else
			{
				for(int i = 0; i < favoriteGameList.Count; i++)
				{
					drawSprite(favoriteGameList[i].getNameSurface(), 200, gameNameY);
					gameNameY += 32;
				}
			}
					
			drawSprite(instructionTextSurface, 8, 458);
			
			SDL.SDL_UpdateWindowSurface(window);
		}
		
		
		public void drawSprite(IntPtr sprite, int x, int y)
		{
			SDL.SDL_Rect pos;
			pos.x = x; pos.y = y;
			pos.w = 600; pos.h = 480;
			
			SDL.SDL_BlitSurface(sprite, IntPtr.Zero, windowSurface, ref pos);
		}
	}
}