use std::net::{self,Ipv4Addr,SocketAddr,SocketAddrV4,TcpStream};
use std::thread;
use std::io::{self,BufRead};

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

fn handle_client(tcpstream : net::TcpStream) { 
    let addr = tcpstream.peer_addr().unwrap();

    //read data
    let mut stream = io::BufReader::new(tcpstream);
    
    let mut first_line = String::new();
    if let Err(_err) = stream.read_line(&mut first_line) {
        panic!("error in reading first line")
    }


    println!("data = {} from {}",first_line,addr);
    
    //send data
}

