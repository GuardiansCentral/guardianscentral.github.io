import React from 'react'
import { NavLink } from 'react-router-dom'

const Crucible = () => {
    return(
        <div>
            <h1>Welcome to the Guardians Central Crucible Page</h1>
            <NavLink to="/">
                <div>To Home Page</div>
            </NavLink>
        </div>
    );
}

export default Crucible