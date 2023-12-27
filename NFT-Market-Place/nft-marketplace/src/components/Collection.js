import { useState, useEffect } from 'react'
import { Row, Col, Card, Button, Form } from 'react-bootstrap'
import {ethers} from 'ethers'
const {utils} = 'ethers';
const Collection = ({ marketplace, axICToken, account }) => {
  const [loading, setLoading] = useState(true)
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
      // console.log(metadata.image);
      let imgURI = "https://ipfs.io/ipfs/" + metadata.image.slice(7);
      //Define item
      const axie = {
        tokenId: a,
        axieId: metadata.name,
        image: imgURI
      }
      // Add item to items array
      tempAxies.push(axie);
      setListAxies(...listAxies, tempAxies);
      setLoading(false)
    });
  }
  const SaleOnMarketplace = async (axie, price) => {
    const ethPrice = utils.parseEther(price.toString())
    await (await marketplace.makeItem(axie.tokenId, axie.axieId, ethPrice)).wait()
    loadUserCollection()
  }
    useEffect(() => {
      if(loading) return(
        <main style={{ padding: "1rem 0" }}>
          <h2>Loading...</h2>
        </main>
      )
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
                  <button onClick={() => SaleOnMarketplace(item)}>Sale</button>
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