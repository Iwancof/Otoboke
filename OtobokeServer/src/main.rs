use std::net::{self,TcpStream};
use std::thread;
use std::io::{self,BufRead,Write};



fn main() { //For one game
    let listener = net::TcpListener::bind("localhost:8080").unwrap();

    for stream in listener.incoming() {
        match stream {
            Ok(stream) => {
                thread::spawn(move || {
                    handle_client(stream)
                });
            }
            Err(_) => {panic!("connection failed"); }
        };
    }
} 

fn handle_client(mut tcpstream : net::TcpStream) { 
    let addr = tcpstream.peer_addr().unwrap();

    //read data
    let mut stream = io::BufReader::new(&tcpstream);
    
    let mut first_line = String::new();
    if let Err(_err) = stream.read_line(&mut first_line) {
        panic!("error in reading first line")
    }


    println!("data = {} from {}",first_line,addr);
    
    //send data
   
    let msg = b"This is reply from rust tcp listener!!";
    tcpstream.write(msg);
    /*
    match TcpStream::connect(addr) {
        Ok(mut stream) => {
            let msg = b"This is reply from rust tcp listener!!";
            stream.write(msg);
        },
        Err(_) => {
            panic!("error in sending message")
        }
    }
    */
}


struct GameController {
    clients : Vec<TcpStream>,
    player_limit : usize,
}

impl GameController {
    pub fn new() -> GameController {
        GameController{clients:Vec::new(),player_limit:1,}
    }
    fn wait_for_players(&self) {
        let listener = net::TcpListener::bind("localhost:8080").unwrap();

        while !(self.clients.len() >= self.player_limit) {
            for stream in listener.incoming() {
                match stream {
                    Ok(stream) => {
                        thread::spawn(move || {
                            //self.player_join(stream)
                            println!("{:?}",stream);
                        });
                    },
                    Err(_) => {
                        println!("Unknown client detected.")
                    }
                }
            }
        }
    }
    fn player_join(&self,mut stream : net::TcpStream) {
        stream.write(format!(r#""counter":{}"#,self.clients.len()).as_bytes());
    }

}



