// package: 
// file: Cherry.proto

var Cherry_pb = require("./Cherry_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var CherryData = (function () {
  function CherryData() {}
  CherryData.serviceName = "CherryData";
  return CherryData;
}());

CherryData.GetDashboardData = {
  methodName: "GetDashboardData",
  service: CherryData,
  requestStream: false,
  responseStream: false,
  requestType: Cherry_pb.DashboardRequest,
  responseType: Cherry_pb.DashboardReply
};

CherryData.GetProducts = {
  methodName: "GetProducts",
  service: CherryData,
  requestStream: false,
  responseStream: false,
  requestType: Cherry_pb.ProductsRequest,
  responseType: Cherry_pb.ProductsReply
};

exports.CherryData = CherryData;

function CherryDataClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

CherryDataClient.prototype.getDashboardData = function getDashboardData(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(CherryData.GetDashboardData, {
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

CherryDataClient.prototype.getProducts = function getProducts(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(CherryData.GetProducts, {
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

exports.CherryDataClient = CherryDataClient;

