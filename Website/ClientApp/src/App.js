import './index.css'

import React from 'react'

import {Navbar, NavbarItem} from './components/Navbar/widget'
import {Home, MainWindow } from './layouts/Home/Home';

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
        <NavbarItem title = "About" location = "/about" />
        <NavbarItem title = "Home" location = "/" />
        <NavbarItem title = "Add bot" location = "/add-bot" />
      </Navbar>
    <Home>
      <MainWindow />
    </Home>
    <footer>This will be the footer.</footer>
  </div>);
}

export default App;
