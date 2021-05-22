import React from 'react';
import PropTypes from 'prop-types';
import './AboutCard.css';
import GenericCard from "../GenericCard/GenericCard";

AboutCard.propTypes = {
    
};

const AboutTitle = () => {
   return (
       <h1>About Sans Car</h1>
   ) 
}

function AboutCard(props) {
    return (
        <GenericCard title={<AboutTitle />} headerStyle="about-header" bodyStyle="about-body" >
    <div className="about-body">
        <h2>What is the purpose?</h2>
        <p>
            Sans car was written for one specific purpose: someone says sans car, someone posts the sans car.
            It's quite a beautiful thing really. Over time, I really wanted to learn software development, so this was
            created in order to give me something to do. Sans car is a toy for me to experiment and have fun with.
        </p>
        <h2>How can I contribute?</h2>
        <p>
            Sans car is open source on <a href="https://github.com/tristan-zander/SansCar">GitHub</a>. If you would like to add or suggest a feature, submit an issue and see
            what people think about it.
        </p>
    </div>
        </GenericCard>
)
}

export default AboutCard;