import React, { useState } from 'react';
import axios from 'axios';
import '../Assets/ImportFile.css'

function FileUpload() {
  const [selectedFile, setSelectedFile] = useState(null);


  const handleFileChange = (event) => {
    setSelectedFile(event.target.files[0]);
  };

  const handleUpload = async () => {

    if (selectedFile == null)
        alert("File can't be empty")

    if (selectedFile) {
      const formData = new FormData();
      formData.append('Orders', selectedFile);

      try {
        const response = await axios.post('http://localhost:5000/ImportOrder', formData, {
          headers: {
            'Content-Type': 'multipart/form-data', // Important!
          },
        });

        if (response.status === 200) {
          window.localStorage.setItem("Orders", JSON.stringify(response.data))
          alert('Success!')
        } else {
          alert('Fail to send orders.')
        }
      } catch (error) {
        console.error('Error uploading file:', error);
        alert('Unable to upload file, verify your connection.', error)
      }
    }
  };

  const handleCalcOrder = async() => {

    var Orders = window.localStorage.getItem("Orders")

    if (Orders == null)
    {
      alert('Please, import the order list first.')
    }

    try {

      const response = await axios.post('http://localhost:5000/OrderHandler', Orders, {
        headers: {
          'Content-Type': 'application/json'
        },
      });

      if (response.status === 200) {

        window.localStorage.setItem("OrdersCalculated", JSON.stringify(response.data))
        alert('Success to calculate orders! Check out the Orders tab!')

      } else {
        alert('Fail to calculate orders!')
      }
    } catch (error) {
      console.error('Error uploading file:', error);
    }
  };

  return (
    <div>

      <h3 className='upload-title'> Drag a file or choose one and click on "Upload" button</h3>

      <div className='drop-container'>
        <input type="file" onChange={handleFileChange} className="inputFile"/>
      </div>

      <div className='div-btns'>
        <button onClick={handleUpload} className='btn-upload'>Upload</button>
        <button onClick={handleCalcOrder} className='btn-calc'>Calculate Orders</button>
      </div>

    </div>
  );
}

export default FileUpload;
