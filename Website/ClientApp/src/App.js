import './index.css'

import React from 'react'

import {Navbar, NavbarItem} from './components/Navbar/Navbar'
import {Home, MainWindow } from './layouts/Home/Home';
import {CommandRotationWidget} from "./components/CommandRotationWidget/CommandRotationWidget";

function App() {
  // GENERAL STRUCTURE
  //
  //   - Navbar
  //   - Router
  //     - load in specific page
  //     - Redirect '/add-bot' to the discord bot add
  //   - Footer

  return (
    <div>
      <Navbar>
        <NavbarItem title = "Home" location = "/" />
        <NavbarItem title = "Add bot" location = "/add-bot" />
        <NavbarItem title = "Log In" location = "/login" />
      </Navbar>
    <Home>
      <MainWindow />
        { // <CommandRotationWidget />
        }
    </Home>
    <footer>This will be the footer.</footer>
  </div>);
}

export default App;
