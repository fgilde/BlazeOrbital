import { Component, OnInit } from '@angular/core';
import {grpc} from "@improbable-eng/grpc-web";
import {ManufacturingData} from "./generated/Manufacturing_pb_service";
import {ProductsRequest, ProductsReply, Product} from "./generated/Manufacturing_pb";
import { ProductModel } from './models/productModel';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  public products: ProductModel[] = []; 

  public ngOnInit() {

    const request = new ProductsRequest();
    request.setMaxcount(30700);
    request.setModifiedsince(-1);


    grpc.unary(ManufacturingData.GetProducts, {
      request: request,
      host: "https://localhost:7087",
      onEnd: res => {
        const { status, statusMessage, headers, message, trailers } = res;
        if (status === grpc.Code.OK && message) {
        var result = message.toObject() as ProductsReply.AsObject;
        this.products = result.productsList.map(p => 
          <ProductModel>({
           name: p.name,
            shop: p.shop
          }));
        }
      }
    });
    

    // grpc.invoke(ManufacturingData.GetProducts, {
    //   request: request,
    //   host: "https://localhost:7087",
    //   onMessage: (message: ProductsReply) => {
    //     var result = message.toObject() as ProductsReply.AsObject;
    //     console.log("one downloaded")

    //     this.products = result.productsList.map(p => 
    //       <ProductModel>({
    //         name: p.name,
    //         shop: p.shop
    //       }));
    //   },
    //   onEnd: (code: grpc.Code, msg: string | undefined, trailers: grpc.Metadata) => {
    //     if (code == grpc.Code.OK) {
    //       console.log("all downloaded")
    //     } else {
    //       console.log("hit an error", code, msg, trailers);
    //     }
    //   }
    // });
  }
}