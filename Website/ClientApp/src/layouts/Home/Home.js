import "./Home.css";

import React from 'react';
import SansCarImage from '../../res/images/sanscar.jpg';
import { CommandRotationWidget } from '../../components/CommandRotationWidget/CommandRotationWidget';
import AboutCard from "../../components/AboutCard/AboutCard";
import ContactCard from "../../components/ContactCard/ContactCard";

export function Home(props) {
    return (
        <div className="home-container">
            {props.children}
        </div>)
}

export function HomeItem(props) {
    return (
        <div className={"home-item "}>
            {props.children}
        </div>
    );
}

export function MainWindow(props) {
    return (
        <div className="main-window">
            <div className="main-card">
            <Header image={SansCarImage} />
            <Body>
            Sans car can manage your quotes, play audio, and
            (in the future) manage roles and channels. Just say "sans car" anywhere in a server or "sans help" for a list of
            commands.
            </Body>
            </div>
            <AboutCard />
            <ContactCard />
        </div>
    );
}

function Header(props) {
    return (
        <div className="main-header">
            <div className="sans-image-container">
                <img src={props.image} alt="The sans car" />
            </div>
            <div className="main-head-title">
                <h1>Sans Car</h1>
                <h2>A meme bot for discord.</h2>
            </div>
        </div>)
}

function Body(props) {
    return (
        <div className="main-body">
            <p style={ {padding: "1rem"} } >{props.children}
            </p>
            <button className="add-sans-car-button">Add Sans Car to your server</button>
        </div>
    )
}

export default { Home, MainWindow, HomeItem }
