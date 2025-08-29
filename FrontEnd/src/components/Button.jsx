import React from 'react';

export default function Button({ type = 'button', children, className = '', ...props }) {
    return (
        <button 
            type={type} 
            className={`btn ${className}`}
            {...props}
        >
            {children}
        </button>
    );
} 