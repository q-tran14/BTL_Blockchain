import { useState, useEffect } from 'react'
import { Row, Col, Card, Button, Form } from 'react-bootstrap'
import {ethers} from 'ethers'
const {utils} = 'ethers';
const Collection = ({ marketplace, axICToken, account }) => {
  const [loading, setLoading] = useState(true)
  const [inputPrice, setInputPrice] = useState('')
  const [listAxies, setListAxies] = useState([])
  
  const loadUserCollection = async () => {
    // Get all token account have  
    const tokens = await axICToken.getAllToken(account);
    // axICToken.balanceOf(account); => 6 check equal getAllToken length
    console.log("Take out token : "+tokens);
    let tempAxies = [];
    tokens.forEach(async (a) => {
      // get uri url from nft contract
      const uri = await axICToken.tokenURI(a);
      // use uri to fetch the nft metadata stored on ipfs 
      const response = await fetch(uri);
      const metadata = await response.json();
      console.log("metadata + "+metadata.image);
      let imgURI = "https://ipfs.io/ipfs/" + metadata.image.slice(7);
      console.log("img URI : "+imgURI);
      //Define item
      const axie = {
        tokenId: a,
        axieId: metadata.name,
        image: imgURI
      }
      // Add item to items array
      tempAxies.push(axie);
      // console.log(axie.tokenId)
      setListAxies(...listAxies, tempAxies);
      console.log(tempAxies.length);
      tempAxies.forEach(axie => console.log(axie.tokenId)
      )
      setLoading(false)
    });
  }
  const InputChange = (event) =>{
    setInputPrice(event.target.value)
  }
  const clickEvent = (axie) =>{
    console.log(inputPrice)
    console.log(axie.tokenId)
    console.log(axie.axieId)
    SaleOnMarketplace(axie,inputPrice) 
  }
  const SaleOnMarketplace = async (axie, price) => {
    var priceAxis = ethers.parseEther(price)
    console.log("ethers.parseEther('0.000000000001') = "+ ethers.parseEther('0.000000000001'))
    console.log("0.000000000001 = "+ priceAxis)
    await (await marketplace.makeItem(axie.tokenId, priceAxis)).wait()
    loadUserCollection()
  }
    useEffect(() => {
      loadUserCollection()
    }, [])
    return (
      <div>
        {listAxies.length > 0 ?
          <div className="flex justify-center">
          <h1>Number of Axies: {listAxies.length}</h1>
          <Row xs={1} md={2} lg={4} className="g-4 py-3">
              {listAxies.map((item, idx) => (
                <Col key={idx} className="overflow-hidden">
                  <Card>
                    <Card.Img variant="top" src={item.image} />
                    <Card.Footer>PriceETH</Card.Footer>
                  </Card>
                  <input
                    type='text'
                    value={inputPrice}
                    onChange={InputChange}
                  />
                  <button onClick= {() => clickEvent(item)}>Sale</button>
                </Col>
              ))}
            </Row>
        </div>
        :(
            <main style={{ padding: "1rem 0" }}>
              <h2>There are no Axies listed for sale</h2>
            </main>
          )
        }
      </div>
    );
  }
export default Collection