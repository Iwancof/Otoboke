use std::net::{self,TcpStream};
use std::io::{Write,Read,BufReader,BufWriter,BufRead};
use std::sync::{mpsc,Arc,Mutex};
use std::thread;
use std::time::Duration;

use super::game;

pub struct GameController {
    clients : Vec<TcpStream>,
    player_limit : usize,
}

impl GameController {
    pub fn new() -> GameController {
        GameController{clients:Vec::new(),player_limit:2}
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

        //self.test_client();
        //print_typename(BufReader::new(&self.clients[0]));
        /*
        let mut streams = vec![];
        for stream in &self.clients {
            let 
            stream.push
        }
        */
        let mut buffer_streams : Arc<Mutex<Vec<BufStream>>>
            = Arc::new(Mutex::new(
            self.clients.iter().map(
            |c|
            BufStream{
                rd : BufReader::new(
                    c.try_clone().unwrap()),
                wr :  BufWriter::new(
                    c.try_clone().unwrap())
            }
        ).collect()
            ));

        loop { //main game loop 
            //for stream in &mut buffer_streams/* : &'static mut Vec<BufStream> */{
            for i in 0..self.clients.len() {
                let cloned = buffer_streams.clone();
                let (sender,receiver) = mpsc::channel();
                
                thread::spawn(move || {
                    let mut locked = cloned.lock().unwrap();

                    let mut ret = String::new();
                    locked[i].rd.read_line(&mut ret).unwrap();
                    sender.send(ret);
                    //println!("test");
                });

                match receiver.recv_timeout(Duration::from_millis(300)) {
                //match receiver.recv() {
                    Ok(s) => { print!("client{} = {}",i,s); }
                    Err(_) => { continue; }
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

pub struct BufStream {
    rd : BufReader<TcpStream>,
    wr : BufWriter<TcpStream>,
}

/*
impl BufStream {
    pub fn from_stream(stream : TcpStream) -> BufStream {
        let ret : BufStream;
        ret.wr = BufWriter::new(stream);
        ret.rd = BufReader::new(stream);

        ret
    }
}
*/

fn print_typename<T>(_ : T) {
    println!("type = {}",std::any::type_name::<T>());
}

