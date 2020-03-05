use std::net::{self,TcpStream};
use std::io::{Write,Read};

use super::game;

pub struct GameController {
    clients : Vec<TcpStream>,
    player_limit : usize,
    
}

impl GameController {
    pub fn new() -> GameController {
        GameController{clients:Vec::new(),player_limit:1}
    }

    pub fn wait_for_players(&mut self) {
        let listener = net::TcpListener::bind("localhost:8080").unwrap();

        for stream in listener.incoming() {
            match stream {
                Ok(stream) => {
                    //thread::spawn(move || {
                        self.player_join(stream)
                        //println!("{:?}",stream);
                    //});
                },
                Err(_) => {
                    println!("Unknown client detected.")
                }
            }
            if self.clients.len() >= self.player_limit {
                println!("Player limit reached.The game will start soon!");
                break;
            }
        }
    }
    fn player_join(&mut self,mut stream : net::TcpStream) {
        let mut json = "{".to_string() + &format!(r#""counter":{}"#,self.clients.len()) + "}|";
        match stream.write(json.as_bytes()) {
            Ok(_) => {
                println!("Player joined! Player details : {:?}",stream);
                self.clients.push(stream);
            }
            Err(_) => {
                println!("Error occured. This stream will ignore");
            }
        }
    }
    pub fn start_game(&mut self,map : game::Map) {
        self.distribute_map(map);

        let mut buff = [0;2048];
        loop { //main game loop 
            for mut client in &self.clients {
                match client.read(&mut buff) {
                    Ok(size) => {
                        if size != 0 {                        
                            client.flush().expect("Error occurred in flushing buffer");
                            println!("{}",String::from_utf8(buff.to_vec()).unwrap());
                        }
                    }
                    Err(_) => {
                        //println!("Error occurred");
                    }
                }
            }
        }
    }

    pub fn distribute_map(&mut self,map : game::Map) {
        //let mut error_clients : Vec<&TcpStream> = vec![];
        let mut error_clients_index : Vec<usize> = vec![];
        let mut count = 0;
        for mut client in &self.clients {
            match client.write(format!("{}",map.map_to_string()).as_bytes()) {
                Ok(_) => {}
                Err(_) => {
                    println!("[Error]Could not send map data. The stream will exclude");
                    error_clients_index.push(count);
                }
            }
            count += 1;
        }
        for e in &error_clients_index {
            self.clients.remove(*e);
        }
        println!("Map distributed");
    }
}

