// package: 
// file: Manufacturing.proto

import * as jspb from "google-protobuf";

export class DashboardRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DashboardRequest.AsObject;
  static toObject(includeInstance: boolean, msg: DashboardRequest): DashboardRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: DashboardRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DashboardRequest;
  static deserializeBinaryFromReader(message: DashboardRequest, reader: jspb.BinaryReader): DashboardRequest;
}

export namespace DashboardRequest {
  export type AsObject = {
  }
}

export class DashboardReply extends jspb.Message {
  getProjectsbookedvalue(): number;
  setProjectsbookedvalue(value: number): void;

  getNextdeliverydueinms(): number;
  setNextdeliverydueinms(value: number): void;

  getStaffonsite(): number;
  setStaffonsite(value: number): void;

  getFactoryuptimems(): number;
  setFactoryuptimems(value: number): void;

  getServicingtasksdue(): number;
  setServicingtasksdue(value: number): void;

  getMachinesstopped(): number;
  setMachinesstopped(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DashboardReply.AsObject;
  static toObject(includeInstance: boolean, msg: DashboardReply): DashboardReply.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: DashboardReply, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DashboardReply;
  static deserializeBinaryFromReader(message: DashboardReply, reader: jspb.BinaryReader): DashboardReply;
}

export namespace DashboardReply {
  export type AsObject = {
    projectsbookedvalue: number,
    nextdeliverydueinms: number,
    staffonsite: number,
    factoryuptimems: number,
    servicingtasksdue: number,
    machinesstopped: number,
  }
}

export class ProductsRequest extends jspb.Message {
  getModifiedsince(): number;
  setModifiedsince(value: number): void;

  getMaxcount(): number;
  setMaxcount(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ProductsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ProductsRequest): ProductsRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: ProductsRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ProductsRequest;
  static deserializeBinaryFromReader(message: ProductsRequest, reader: jspb.BinaryReader): ProductsRequest;
}

export namespace ProductsRequest {
  export type AsObject = {
    modifiedsince: number,
    maxcount: number,
  }
}

export class ProductsReply extends jspb.Message {
  clearProductsList(): void;
  getProductsList(): Array<Product>;
  setProductsList(value: Array<Product>): void;
  addProducts(value?: Product, index?: number): Product;

  getModifiedcount(): number;
  setModifiedcount(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ProductsReply.AsObject;
  static toObject(includeInstance: boolean, msg: ProductsReply): ProductsReply.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: ProductsReply, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ProductsReply;
  static deserializeBinaryFromReader(message: ProductsReply, reader: jspb.BinaryReader): ProductsReply;
}

export namespace ProductsReply {
  export type AsObject = {
    productsList: Array<Product.AsObject>,
    modifiedcount: number,
  }
}

export class Product extends jspb.Message {
  getId(): string;
  setId(value: string): void;

  getModifiedticks(): number;
  setModifiedticks(value: number): void;

  getCategory(): string;
  setCategory(value: string): void;

  getSubcategory(): string;
  setSubcategory(value: string): void;

  getName(): string;
  setName(value: string): void;

  getLocation(): string;
  setLocation(value: string): void;

  getImage(): string;
  setImage(value: string): void;

  getBrand(): string;
  setBrand(value: string): void;

  getTargeturl(): string;
  setTargeturl(value: string): void;

  getThumbnail(): string;
  setThumbnail(value: string): void;

  getProduct(): string;
  setProduct(value: string): void;

  getSalepercentage(): string;
  setSalepercentage(value: string): void;

  getPrice(): string;
  setPrice(value: string): void;

  getSaleprice(): string;
  setSaleprice(value: string): void;

  getShop(): string;
  setShop(value: string): void;

  getDatecreated(): number;
  setDatecreated(value: number): void;

  getDateupdated(): number;
  setDateupdated(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Product.AsObject;
  static toObject(includeInstance: boolean, msg: Product): Product.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: Product, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Product;
  static deserializeBinaryFromReader(message: Product, reader: jspb.BinaryReader): Product;
}

export namespace Product {
  export type AsObject = {
    id: string,
    modifiedticks: number,
    category: string,
    subcategory: string,
    name: string,
    location: string,
    image: string,
    brand: string,
    targeturl: string,
    thumbnail: string,
    product: string,
    salepercentage: string,
    price: string,
    saleprice: string,
    shop: string,
    datecreated: number,
    dateupdated: number,
  }
}

