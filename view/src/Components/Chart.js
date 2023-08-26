import React, { useState, useEffect } from 'react';
import ApexCharts from 'react-apexcharts';
import axios from "axios";

export default function Chart() {

    const [order, setOrder] = useState('');

    useEffect(() => {
        axios
          .get("http://localhost:5000/GetOrders")
          .then((res) => setOrder(res.data))
          .catch(err => {
            console.log(err)
      });
    }, []);

    const series = [{
        data: []
    }]

    for( let i = 0 ; i < order.length; i++ )
    {
        series[0].data.push({
            x: order[i].corporateName,
            y: order[i].priceWithDelivery
        })
    }

    const options = {
        chart: {
            type: 'bar'
          },
          plotOptions: {
            bar: {
              horizontal: true
            }
          },
    }

    if (order.length > 0)
    {
        return (
            <>
                <ApexCharts
                    options={options}
                    series={series}
                    type="bar"
                    width={640}
                    height={400}
                    marginLeft={500}
                    align="center"
                />
            </>
            
        )
    }
}