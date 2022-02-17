// package: 
// file: Cherry.proto

import * as Cherry_pb from "./Cherry_pb";
import {grpc} from "@improbable-eng/grpc-web";

type CherryDataGetDashboardData = {
  readonly methodName: string;
  readonly service: typeof CherryData;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof Cherry_pb.DashboardRequest;
  readonly responseType: typeof Cherry_pb.DashboardReply;
};

type CherryDataGetProducts = {
  readonly methodName: string;
  readonly service: typeof CherryData;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof Cherry_pb.ProductsRequest;
  readonly responseType: typeof Cherry_pb.ProductsReply;
};

export class CherryData {
  static readonly serviceName: string;
  static readonly GetDashboardData: CherryDataGetDashboardData;
  static readonly GetProducts: CherryDataGetProducts;
}

export type ServiceError = { message: string, code: number; metadata: grpc.Metadata }
export type Status = { details: string, code: number; metadata: grpc.Metadata }

interface UnaryResponse {
  cancel(): void;
}
interface ResponseStream<T> {
  cancel(): void;
  on(type: 'data', handler: (message: T) => void): ResponseStream<T>;
  on(type: 'end', handler: (status?: Status) => void): ResponseStream<T>;
  on(type: 'status', handler: (status: Status) => void): ResponseStream<T>;
}
interface RequestStream<T> {
  write(message: T): RequestStream<T>;
  end(): void;
  cancel(): void;
  on(type: 'end', handler: (status?: Status) => void): RequestStream<T>;
  on(type: 'status', handler: (status: Status) => void): RequestStream<T>;
}
interface BidirectionalStream<ReqT, ResT> {
  write(message: ReqT): BidirectionalStream<ReqT, ResT>;
  end(): void;
  cancel(): void;
  on(type: 'data', handler: (message: ResT) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'end', handler: (status?: Status) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'status', handler: (status: Status) => void): BidirectionalStream<ReqT, ResT>;
}

export class CherryDataClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  getDashboardData(
    requestMessage: Cherry_pb.DashboardRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: Cherry_pb.DashboardReply|null) => void
  ): UnaryResponse;
  getDashboardData(
    requestMessage: Cherry_pb.DashboardRequest,
    callback: (error: ServiceError|null, responseMessage: Cherry_pb.DashboardReply|null) => void
  ): UnaryResponse;
  getProducts(
    requestMessage: Cherry_pb.ProductsRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: Cherry_pb.ProductsReply|null) => void
  ): UnaryResponse;
  getProducts(
    requestMessage: Cherry_pb.ProductsRequest,
    callback: (error: ServiceError|null, responseMessage: Cherry_pb.ProductsReply|null) => void
  ): UnaryResponse;
}

