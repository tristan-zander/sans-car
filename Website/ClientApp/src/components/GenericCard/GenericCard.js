import React from 'react';
import PropTypes from 'prop-types';
import css from './GenericCard.module.css';

GenericCard.propTypes = {
    title: PropTypes.element,
    children: PropTypes.element.isRequired,
    headerStyle: PropTypes.string,
    bodyStyle: PropTypes.string
};

function GenericCard(props) {
    console.log(props)
    return (
        <div>
            {props.title}
            <div className={css.body + " " + props.bodyStyle}>{props.children}</div>
        </div>
    );
}

export default GenericCard;