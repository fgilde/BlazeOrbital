// package: 
// file: Manufacturing.proto

import * as Manufacturing_pb from "./Manufacturing_pb";
import {grpc} from "@improbable-eng/grpc-web";

type ManufacturingDataGetDashboardData = {
  readonly methodName: string;
  readonly service: typeof ManufacturingData;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof Manufacturing_pb.DashboardRequest;
  readonly responseType: typeof Manufacturing_pb.DashboardReply;
};

type ManufacturingDataGetProducts = {
  readonly methodName: string;
  readonly service: typeof ManufacturingData;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof Manufacturing_pb.ProductsRequest;
  readonly responseType: typeof Manufacturing_pb.ProductsReply;
};

export class ManufacturingData {
  static readonly serviceName: string;
  static readonly GetDashboardData: ManufacturingDataGetDashboardData;
  static readonly GetProducts: ManufacturingDataGetProducts;
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

export class ManufacturingDataClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  getDashboardData(
    requestMessage: Manufacturing_pb.DashboardRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: Manufacturing_pb.DashboardReply|null) => void
  ): UnaryResponse;
  getDashboardData(
    requestMessage: Manufacturing_pb.DashboardRequest,
    callback: (error: ServiceError|null, responseMessage: Manufacturing_pb.DashboardReply|null) => void
  ): UnaryResponse;
  getProducts(
    requestMessage: Manufacturing_pb.ProductsRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: Manufacturing_pb.ProductsReply|null) => void
  ): UnaryResponse;
  getProducts(
    requestMessage: Manufacturing_pb.ProductsRequest,
    callback: (error: ServiceError|null, responseMessage: Manufacturing_pb.ProductsReply|null) => void
  ): UnaryResponse;
}

