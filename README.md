## What is NXO?

NXO refers to 'n-dimensional Tic-Tac-Toe', an extension of traditional tic tac toe into a generic number of dimensions. 
The goal of this project is simply to provide a way to play this modified game with your friends.
This is <b>not</b> a production version, as there are far more security (also known as *any level of security*) and scalability measures to take, but it acts as a *proof of concept* nonetheless.

### Features

This system provides the following:

  1. Synchronous online play with your friends
  2. Different bots to play against (implemented with different algorithms and playstyles)
  3. A no-account lobby authentication system (inspired by [Jackbox](https://jackbox.tv))

### Technologies

The following technologies are utilized:

  1. The backend web app is built on ASP.Net Core in C#, with a (possibly unecessary) level of abstraction between the game specific logic and the web hosting itself. The original idea behind this was so we could add additional variations/games into this system later on (which may or may not happen).
  2. The frontend is written in Blazor WebAssembly in C#. A large part of the motivation for this project for (Rob) was to see how well Blazor works with traditional ASP.Net Web Applications (coming from a traditional MVC background). 
  3. For synchronous actions, we utilize WebSockets (using SignalR specifically due to the existance of libraries for Blazor WASM).  

### Game Description

The idea is that you play the same way that you would play regular tic-tac-toe, but *the win conditions are spread across many more dimensions*. It is somewhat difficult to describe *exactly* what happens in each  

More mathematical and gameplay details can be found on the [About](https://nxoproject.azurewebsites.net/about) page.

## The Team

<b>Robert Scheidegger :rocket::bulb:</b> - Project lead and Full-Stack engineer

<b>James Billings :boom: :star: </b> - Bot algorithms and AI

<b>Erik Swanson :black_nib::triangular_ruler: </b> - Design and CSS Wizardry :crystal_ball:  