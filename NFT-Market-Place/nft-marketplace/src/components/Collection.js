import { useState, useEffect } from 'react'
import {ethers} from 'ethers';
import { Row, Col, Card, Button, Form } from 'react-bootstrap'
import { create as ipfsHttpClient } from 'ipfs-http-client'
const client = ipfsHttpClient('https://ipfs.infura.io:5001/api/v0')
const {utils} = 'ethers';

function renderSoldItems(items) {
  return (
    <>
      <h2>Sold</h2>
      <Row xs={1} md={2} lg={4} className="g-4 py-3">
        {items.map((item, idx) => (
          <Col key={idx} className="overflow-hidden">
            <Card>
              <Card.Img variant="top" src={item.image} />
              <Card.Footer>
                For {utils.formatEther(item.totalPrice)} ETH - Recieved {utils.formatEther(item.price)} ETH
              </Card.Footer>
            </Card>
          </Col>
        ))}
      </Row>
    </>
  )
}
const Collection = ({ marketplace, axICToken, account }) => {
  const [loading, setLoading] = useState(true)
  const [listAxies, setListAxies] = useState([])
  const loadUserCollection = async () => {
    // Get all token account have  
    const tokens = await axICToken.getAllToken(account);
    // console.log(tokens);
    let tempAxies = [];
    tokens.forEach(async (a) => {
      // get uri url from nft contract
      const uri = await axICToken.tokenURI(a);
      // use uri to fetch the nft metadata stored on ipfs 
      const response = await fetch(uri);
      const metadata = await response.json();
      // console.log(metadata.image);
      let imgURI = "https://ipfs.io/ipfs/" + metadata.image.slice(7);
      //Define item
      const axie = {
        tokenId: a,
        axieId: metadata.name,
        image: imgURI
      }
      // console.log(axie);
      // Add item to items array
      tempAxies.push(axie);
      setListAxies(tempAxies);
    });
    setLoading(false)
  }
    useEffect(() => {
      loadUserCollection()
    }, [])
    if (loading) return (
      <main style={{ padding: "1rem 0" }}>
        <h2>Loading...</h2>
      </main>
    )
    // return (
    //   <div className="flex justify-center">
    //   <h1>{axies.length}</h1>
    //     {axies.length > 0 ?
    //       axies.map((axie) => (
    //       <div className="axieItem">
    //         <div className="axieImg">
    //         <h1></h1>
    //           <img id="axieImg" src={axie.image} alt=''/>
    //         </div>
    //         <div className="axieInfo">
    //           <h3 id="axieID">{axie.name}</h3>
    //         </div>
    //         <Button className="buy-sellBtn" onClick="" variant="outline-light">Sell</Button>
    //       </div>
    //     )) : (
    //       <main style={{ padding: "1rem 0" }}>
    //           <h2>No listed assets</h2>
    //       </main>
    //     )}
    //   </div>
    // );
    return (
      <div className="flex justify-center">
      <div>
      <h1>{listAxies.length}</h1>
        {listAxies.map(axie=> (
          <div key={axie.tokenId}> 
            <img src={axie.image}></img>
            <h1>{axie.axieId}</h1>
            <h2>{axie.tokenId}</h2>
          </div>
        ))}
      </div>
      </div>
    );
  }
export default Collection