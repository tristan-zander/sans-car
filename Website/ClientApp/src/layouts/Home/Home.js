import "./Home.css";

import React from 'react';
import SansCarImage from '../../res/images/sanscar.jpg';
import { CommandRotationWidget } from '../../components/CommandRotationWidget/widget';

export function Home(props) {
    return (
        <div className="home-container">
            {props.children}
            <CommandRotationWidget />
        </div>)
}

export function HomeItem(props) {
    return (
        <div className={"home-item "}>
            {props.children}
        </div>
    );
}

export function MainWindow() {
    return (
        <div className="main-window">
            <div className="sans-decorator">
            <div className="main-window-upper-half">
                <div className="sans-image-container">
                    <img src={SansCarImage} alt="The sans car" />
                    <h1>Sans Car</h1>
                </div>
            </div>
            <div className="main-window-lower-half">
                <p style={ {padding: "1rem"} } >Sans car is a meme bot for Discord. Just say "sans car" anywhere in a server!</p>
                <button className="add-sans-car-button">Add Sans Car to your server</button>
            </div>
            </div>
        </div>
    );
}

export default { Home, MainWindow, HomeItem }
