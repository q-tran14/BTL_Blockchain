import React, { useState } from 'react';
import { ethers } from "ethers";
import {BrowserRouter, Routes, Route} from 'react-router-dom';
import Navigation from './components/NavigationBar.js';
import Marketplace from './components/Marketplace.js';
import Collection from './components/Collection.js';
import { Spinner } from 'react-bootstrap';
import '../src/css/App.css'
import marketplaceAbi from '../src/SmartContract/build/contracts/Marketplace.json';
import axICTokenAbi from '../src/SmartContract/build/contracts/AxICToken.json';


function App() {
  const [account, setAccount] = useState(null);
  const [loading, setLoading] = useState(null);
  const [balance, setBalance] = useState('');
  const [axICToken, setAxICToken] = useState({});
  const [marketplace, setMarketplace] = useState({});

  // Smart Contract address - new address
  const axICTokentAddress = "0xa4A3182D250D204A343A45a62b018A0eEcFcc1D5";
  const marketplaceAddress = "0xE464C70BbdBb5122f6B40Ded3BB97B0314E9Fa99";

  
  //Show smart contract information
  //console.log('Marketplace abi:');
  //console.log(marketplaceAbi.abi);
  //console.log('NFT abi:');
  //console.log(axICTokenAbi.abi);

  const loadContracts = async (signer) => {
    // Create smart contract instance
    const marketplace = new ethers.Contract(marketplaceAddress,marketplaceAbi.abi,signer);
    const axICToken = new ethers.Contract(axICTokentAddress, axICTokenAbi.abi, signer);
    // Set smart contract
    setAxICToken(axICToken);
    setMarketplace(marketplace);
    setLoading(false);
  };

  const web3Handler = async () => {
    await window.ethereum.request({ method: 'eth_requestAccounts' });
    // Get provider from Metamask
    const provider = new ethers.BrowserProvider(window.ethereum);
    // Set signer
    const signer = await provider.getSigner();
    // Get account address
    const account = await signer.address;
    setAccount(account);
    // Get account balance
    const balance = await signer.balance;
    setBalance(balance);
    // Load smart contract
    loadContracts(signer);
  }
  return (
      <BrowserRouter>
        <div className='bg-full'>
          <Navigation web3Handler={web3Handler} account={account}/>
          <div className="flex justify-center body-page">
            {loading ? 
            (
                  <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '80vh' }}>
                    <Spinner animation="border" style={{ display: 'flex' }} />
                    <p className='mx-3 my-0'>Awaiting Metamask Connection...</p>
                  </div>
                ) : (
              <><Routes>
                      <Route path="/Marketplace" element={<Marketplace marketplace={marketplace} axICToken={axICToken} />} />
                      <Route path="/Collection" element={<Collection marketplace={marketplace} axICToken={axICToken} account={account} />} />

                  </Routes>
              </>
            )
           }
          </div>
        </div>
      </BrowserRouter>
  );
}

export default App;