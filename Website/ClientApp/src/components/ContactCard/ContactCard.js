import React from 'react';
import './ContactCard.css';

function ContactCard(props) {
    return (
        <div className="contact-card">
            <div>
                <h1>Contact</h1>
            </div>
            <div>
                <ContactField title="Github" link="https://github.com/tristan-zander">H</ContactField>
            </div>
        </div>
    )
}

function ContactField(props) {
    return (
        <div>
            <h2>{props.link && <a href={props.link}>{props.title}</a> || props.title}</h2>
            <p>{props.children}</p>
        </div>
    )
}

export default ContactCard;