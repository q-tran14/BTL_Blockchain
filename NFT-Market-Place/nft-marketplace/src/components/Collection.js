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
  const [items, setItems] = useState([])
  const [solds, setSolds] = useState(false)

  const loadListedItems = async () => {
    // Load all sold items that the user listed
    const itemCount = await marketplace.itemCount
    let listItems = []
    let soldItems = []
    for (let index = 1; index <= itemCount; index++) {
      const i = await marketplace.items(index)
      if (i.seller.toLowerCase() === account) {
        // get uri url from nft contract
        const uri = await axICToken.tokenURI(i.tokenId)
        // use uri to fetch the nft metadata stored on ipfs 
        const response = await fetch(uri)
        const metadata = await response.json()
        // get total price of item (item price + fee)
        const totalPrice = await marketplace.getTotalPrice(i.itemId)
        // define listed item object
        let item = {
          totalPrice,
          price: i.price,
          itemId: i.itemId,
          name: metadata.name,
          description: metadata.description,
          image: metadata.image
        }
        listItems.push(item)
        // Add listed item to sold items array if sold
        if (i.sold) soldItems.push(item)
      }
    }
    setLoading(false)
    setItems(listItems)
    setSolds(soldItems)
  }
  const SellItem = async (item) => {
    await (await marketplace.makeItem(item.itemId, item.price)).wait()
    loadListedItems()
  }
    

    useEffect(() => {
      loadListedItems()
    }, [])
    if (loading) return (
      <main style={{ padding: "1rem 0" }}>
        <h2>Loading...</h2>
      </main>
    )
    return (
      <div className="flex justify-center">
        {items.length > 0 ?
          <div className="px-5 py-3 container">
              <h2>Listed</h2>
            <Row xs={1} md={2} lg={4} className="g-4 py-3">
              {items.map((item, idx) => (
                <Col key={idx} className="overflow-hidden">
                  <Card>
                    <Card.Img variant="top" src={item.image} />
                    <Card.Footer>{utils.formatEther(item.totalPrice)} ETH</Card.Footer>
                    <div className='d-grid'>
                        <Button onClick={() => SellItem(item)} variant="primary" size="lg">
                          Sale {ethers.formatEther(item.totalPrice)} ETH
                        </Button>
                      </div>
                  </Card>
                </Col>
              ))}
            </Row>
              {solds.length > 0 && renderSoldItems(solds)}
          </div>
          : (
            <main style={{ padding: "1rem 0" }}>
              <h2>No listed assets</h2>
            </main>
          )}
      </div>
    );
  }
export default Collection