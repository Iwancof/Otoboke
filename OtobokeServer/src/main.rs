use std::net;
use std::thread;
use std::io;

fn main() {
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

fn handle_client(stream : net::TcpStream) {
    let mut stream = io::BufReader::new(stream);

    let mut first_line = String::new();
    if let Err(err) = stream.read_line(&mut first_line) {
        panic!("error in reading first line");
    }

    let mut params = first_line.split_whitespace();
    
    for e in params {
        println!("{}",e);
    }
}

