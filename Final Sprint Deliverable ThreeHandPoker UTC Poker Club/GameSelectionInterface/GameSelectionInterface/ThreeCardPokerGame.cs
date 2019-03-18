/*
 * Created by SharpDevelop.
 * User: Richard
 * Date: 11/17/2017
 * Time: 1:03 AM
 */
using System;
using SDL2;
using ThreeCardPoker;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace sdlTest
{
	//An individual game of ThreeCardPoker
	//This class is primarily a graphical interface to the logic of the ThreeCardPoker project 
	public class ThreeCardPokerGame
	{
		Game game = new Game();
		
		SDL.SDL_Event e;
		bool exitGame = false;
		
		IntPtr window;
		IntPtr windowSurface;
		uint clearColor;
		SDL.SDL_Color textColor;
		
		SDL.SDL_Rect screenRect;
		
		IntPtr cardSprite;
		IntPtr cardBackSprite;
		IntPtr playButton;
		IntPtr foldButton;
		IntPtr dealButton;
		IntPtr quitButton;
				
		IntPtr font;
		IntPtr balanceTextSprite;
		IntPtr betTextSprite;
		IntPtr playerWinTextSprite;
		IntPtr dealerWinTextSprite;
		IntPtr drawTextSprite;
		
		IntPtr debugPlayerHandSprite;
		IntPtr debugDealerHandSprite;
			
		//initialize the Three Card Poker sprites and variables
		public unsafe ThreeCardPokerGame(IntPtr interfaceWindow)
		{
			window = interfaceWindow;
			windowSurface = SDL.SDL_GetWindowSurface(window);
			
			SDL.SDL_Surface* windowSDLSurface = (SDL.SDL_Surface*)windowSurface;
			clearColor = SDL.SDL_MapRGB(windowSDLSurface->format, 0, 100, 0);
			textColor.r = 255; textColor.b = 255; textColor.g = 255; textColor.a = 255;
			
			screenRect.x = 0; screenRect.y = 0;
			screenRect.w = 600; screenRect.h = 480;
			
			cardSprite = SDL.SDL_LoadBMP("cards.bmp");
			cardBackSprite = SDL.SDL_LoadBMP("card back blue.bmp");
			playButton = SDL.SDL_LoadBMP("play button.bmp");
			foldButton = SDL.SDL_LoadBMP("fold button.bmp");
			dealButton = SDL.SDL_LoadBMP("deal button.bmp");
			quitButton = SDL.SDL_LoadBMP("quit button.bmp");
			
			font = SDL_ttf.TTF_OpenFont("times.ttf", 20); 
			betTextSprite = SDL_ttf.TTF_RenderText_Solid(font, "Ante: $50", textColor);
			playerWinTextSprite = SDL_ttf.TTF_RenderText_Solid(font, "You won!", textColor);
			dealerWinTextSprite = SDL_ttf.TTF_RenderText_Solid(font, "The dealer won.", textColor);
			drawTextSprite = SDL_ttf.TTF_RenderText_Solid(font, "Draw...", textColor);
		}
		
		
		public void run()
		{
			game.Play(5000, 50);
			game.Deal();
						
			while(exitGame == false)
			{
				while(SDL.SDL_PollEvent(out e) == 1)
				{
					//handle closing the window
					if (e.type == SDL.SDL_EventType.SDL_QUIT)
					{
						exitGame = true;
					}
					//handle mouse clicks
					else if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
					{
						int mouseX, mouseY;
						SDL.SDL_GetMouseState(out mouseX, out mouseY);
						
						//if the user clicked the exit button
						if (checkClick(mouseX, mouseY, 500, 0))
						{
							exitGame = true;
						}
						
						//if the user pressed the deal button
						if ((game.AllowedActions & GameAction.Deal) == GameAction.Deal)
			            {
			                if (checkClick(mouseX, mouseY, 200, 300))
							{
			                	game.Deal();
			                	continue; //continue to next part of while loop
			                	//(or else one click might press multiple buttons)
							}
			            }
						
						//if the player hits the "fold" button 
						//(In the game logic, the fold command is actually called "hit")
						//(because the logic was based off blackjack logic)
						//(this should eventually be changed)
						if ((game.AllowedActions & GameAction.Hit) == GameAction.Hit)
			            {
			                if (checkClick(mouseX, mouseY, 250, 300))
							{
			                	game.Hit();
			                	continue;
							}
			            }
					
						//if the player hits the "play" button	
			            if ((game.AllowedActions & GameAction.Stand) == GameAction.Stand)
			            {
			                if (checkClick(mouseX, mouseY, 150, 300))
							{
			                	//in three card poker, playing involves matching your ante bet
			                	//(which effectively doubles your bet)
			                	game.Player.Bet *= 2;
			                	game.Stand();
			                	game.Player.Bet /= 2;
			                	continue;
							}
			            }
			
					}
				}
				
				draw();
				
				
				SDL.SDL_Delay(30);
			}
		}
		
		private void draw()
		{
			//clear the screen
			SDL.SDL_FillRect(windowSurface, ref screenRect, clearColor);
			
			//draw the dealers cards
			ReadOnlyCollection<Card> dealerCards = game.Dealer.Hand.Cards;
			for(int i = 0; i < 3; i++)
			{
				if (dealerCards[i].IsFaceUp)
				{
					drawCard(dealerCards[i].Suite, dealerCards[i].Rank, 100 + 120*i, 50);
				}
				else
				{
					drawSprite(cardBackSprite, 100 + 120*i, 50);
				}
			}
			
			//draw the player's cards
			ReadOnlyCollection<Card> playerCards = game.Player.Hand.Cards;
			for(int i = 0; i < 3; i++)
			{
				drawCard(playerCards[i].Suite, playerCards[i].Rank, 100 + 120*i, 175);
			}
			
			//draw the buttons which may be pressed
			if ((game.AllowedActions & GameAction.Hit) == GameAction.Hit)
            {
                drawSprite(playButton, 150, 300);
            }
            if ((game.AllowedActions & GameAction.Stand) == GameAction.Stand)
            {
               	drawSprite(foldButton, 250, 300);
            }
            if ((game.AllowedActions & GameAction.Deal) == GameAction.Deal)
            {
                drawSprite(dealButton, 200, 300);
            }
			
            drawSprite(quitButton, 500, 0);
            
            drawSprite(betTextSprite, 0, 450);
            
            string balanceText = "Your balance: " + game.Player.Balance.ToString();
			balanceTextSprite = SDL_ttf.TTF_RenderText_Solid(font, balanceText, textColor);
			drawSprite(balanceTextSprite, 200, 450);
			
			if (game.LastState == GameState.DealerWon)
			{
				drawSprite(dealerWinTextSprite, 200, 400);
			}
			else if (game.LastState == GameState.PlayerWon)
			{
				drawSprite(playerWinTextSprite, 200, 400);
			}
			else if (game.LastState == GameState.Draw)
			{
				drawSprite(drawTextSprite, 200, 400);
			}
			
			
			//these 4 lines of code write the scores of the player's hand and the dealer's hand onto the screen
			//this can be used to debug the hand scoring algorithm
			
			//debugDealerHandSprite = SDL_ttf.TTF_RenderText_Solid(font, game.Dealer.Hand.getScore.ToString(), textColor);
			//drawSprite(debugDealerHandSprite, 180, 400);
			//debugPlayerHandSprite = SDL_ttf.TTF_RenderText_Solid(font, game.Player.Hand.getScore.ToString(), textColor);
			//drawSprite(debugPlayerHandSprite, 180, 420);
			                        
			SDL.SDL_UpdateWindowSurface(window);
		}
		
		private void drawCard(Suite suite, Rank rank, int x, int y)
		{
			SDL.SDL_Rect pos;
			pos.x = x; pos.y = y;
			pos.w = 600; pos.h = 480;
			
			SDL.SDL_Rect tile;
			tile.x = ((byte)rank - 1)*73; tile.y = ((byte)suite - 1)*98;
			tile.w = 74; tile.h = 98;
			
			SDL.SDL_BlitSurface(cardSprite, ref tile, windowSurface, ref pos);
		}
		
		
		private void drawSprite(IntPtr sprite, int x, int y)
		{
			SDL.SDL_Rect pos;
			pos.x = x; pos.y = y;
			pos.w = 600; pos.h = 480;
			
			SDL.SDL_BlitSurface(sprite, IntPtr.Zero, windowSurface, ref pos);
		}
		
		//check if a mouse click collides with a button
		private bool checkClick(int clickX, int clickY, int buttonX, int buttonY)
		{
			if (clickX < buttonX)
			{
				return false;
			}
			if (clickY < buttonY)
			{
				return false;
			}
			if (clickX > (buttonX + 96))
		    {
				return false;
		    }
			if (clickY > (buttonY + 96))
		    {
				return false;
		    }
			return true;
		}
	}
}
