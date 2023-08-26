import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../Assets/Table.css'


function normalizeDateTime(date)
{
    var DateTime = date.substr(0, 10).split('-')
    var year = DateTime[0]
    var month = DateTime[1]
    var day = DateTime[2]

    var DateParsed = `${day}/${month}/${year}`
    return DateParsed;
}

function formatCurrencyBRL(price)
{
    return `R$ ${price}`
}

const TableComponent = () => {

    const [data, setData] = useState([]);

    useEffect(() => {
    const storedData = JSON.parse(localStorage.getItem('OrdersCalculated')) || [];
    setData(storedData);
    }, []);

    const PostNewOrders = async() => {

        var Orders = window.localStorage.getItem("OrdersCalculated")
    
        if (Orders == null)
        {
          alert('Please, import the order list first.')
        }
    
        try {
    
          const response = await axios.post('http://localhost:5000/ApproveOrders', Orders, {
            headers: {
              'Content-Type': 'application/json'
            },
          });
    
          if (response.status === 200) {
            alert('Success to calculate orders! Check out the Orders tab!')
          } else {
            alert('Fail to calculate orders!')
          }
        } catch (error) {
          console.error('Error uploading file:', error);
        }
      };

    if (data.length > 0)
    {
        return (
            <>
                <table class="styled-table">
                <thead>
                    <tr>
                    <th>Customer</th>
                    <th>Product</th>
                    <th>Zip Code</th>
                    <th>Order Number</th>
                    <th>Price With Delivery</th>
                    <th>Price Without Delivery</th>
                    <th>Date Ordered</th>
                    <th>Estimated Delivery</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((item, index) => (
                        <tr key={index}>
                            <td>{item.corporateName}</td>
                            <td>{item.product}</td>
                            <td>{item.zipCode}</td>
                            <td>{item.orderNumber}</td>
                            <td>{formatCurrencyBRL(item.priceWithDelivery)}</td>
                            <td>{formatCurrencyBRL(item.priceWithoutDelivery)}</td>
                            <td>{normalizeDateTime(item.dateOrdered)}</td>
                            <td>{normalizeDateTime(item.estimatedDelivery)}</td>
                        </tr>
                    ))}
                </tbody>
                </table>

                <button className='btn-approve' onClick={PostNewOrders}> Approve orders now! </button>
            </>
        );
    }
}

export default TableComponent;