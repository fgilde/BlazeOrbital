// package: 
// file: Manufacturing.proto

var Manufacturing_pb = require("./Manufacturing_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var ManufacturingData = (function () {
  function ManufacturingData() {}
  ManufacturingData.serviceName = "ManufacturingData";
  return ManufacturingData;
}());

ManufacturingData.GetDashboardData = {
  methodName: "GetDashboardData",
  service: ManufacturingData,
  requestStream: false,
  responseStream: false,
  requestType: Manufacturing_pb.DashboardRequest,
  responseType: Manufacturing_pb.DashboardReply
};

ManufacturingData.GetProducts = {
  methodName: "GetProducts",
  service: ManufacturingData,
  requestStream: false,
  responseStream: false,
  requestType: Manufacturing_pb.ProductsRequest,
  responseType: Manufacturing_pb.ProductsReply
};

exports.ManufacturingData = ManufacturingData;

function ManufacturingDataClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

ManufacturingDataClient.prototype.getDashboardData = function getDashboardData(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(ManufacturingData.GetDashboardData, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

ManufacturingDataClient.prototype.getProducts = function getProducts(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(ManufacturingData.GetProducts, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

exports.ManufacturingDataClient = ManufacturingDataClient;

