import '../../index.css';
import "./style.css";

import React from 'react'

export function Navbar(props) {
  return (<nav className = "navbar">
          <ul className = "navbar-nav">{props.children}</ul>
        </nav>)
}

// title, location
export function NavbarItem(props) {
    const onClick = () => {
        console.log('unimplemented!')
    }

    return(
        <li className="nav-item" onClick={onClick}>{props.title}</li>
    )
}

export default { Navbar, NavbarItem }
