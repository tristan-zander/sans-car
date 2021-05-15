import React, { useEffect, useState } from 'react'
import './CommandRotationWidget.css'

const tempCommands = {
	'ping': {
		'description': 'Get the time it takes for the bot to respond to your messages.'
		// Automatically determine usage?
	},
	'play': {
		'description': 'Play songs on YouTube or Soundcloud, or just any mp3 file.'
	},
	'quote': {
		'description': 'Get quotes from your server and add new ones.',
		'subcommands': [
			{
				'name': 'add',
				'description': 'add a quote'	
			},
			{
				'name': 'delete',
				'description': 'delete a specified quote.'
			}
			]
	}
}
Object.freeze(tempCommands)

export function CommandRotationWidget(props) {
	// While we get commands, enter a loading state. Update it with callback.
	// getCommands().then().catch()
	
	let commands = Object.keys(tempCommands).map(key => {
		return (
			<div className="command">
				<h1>{key.toUpperCase()}</h1>
				<p>{tempCommands[key].description}</p>
				{ tempCommands[key].subcommands?.map(subc => <div><h2>{subc.name}</h2><p>{subc.description}</p></div>)  }
			</div>
		)	
	})
	
	let [index, setIndex] = useState(0)
	
	useEffect(() => {
		// Cycle command every 3 secs
		let timeout = setTimeout(() => {
			setIndex((index + 1) % commands.length)
		}, 5000)
		
		return () => {
			clearTimeout(timeout)
		}
	}, [index, commands])
	
	
	// TODO name the variables better.
	return ( 
		<div className="rotation-background">
			{ commands[index] }
		</div>
	)
}

// TODO fill this out once the backend is finished.
async function getCommands() {
	// Query the available commands from the server.
	
	// TODO: stop hard-coding the server api place.
	try {
		// let commands = await fetch('https://sanscar.net/api/v1/commands/available')
	} catch {

	}
}

export default { CommandRotationWidget };
