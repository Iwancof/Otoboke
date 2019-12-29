use std::net::{self,TcpStream};
use std::io::{Write,Read};

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
        match stream.write(format!(r#""counter":{}"#,self.clients.len()).as_bytes()) {
            Ok(_) => {
                println!("Player joined! Player details : {:?}",stream);
                self.clients.push(stream);
            }
            Err(_) => {
                println!("Error occured. It will ignore");
            }
        }
    }
    pub fn start_game(&mut self) {
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
}

