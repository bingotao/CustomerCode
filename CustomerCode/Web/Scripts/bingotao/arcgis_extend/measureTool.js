/*
地图量算工具
start方法启动量算，参数包括point、length、area，默认point（显示坐标）。
*/
define(
    'extend/measureTool',
    [
        'dojo/_base/declare',
        'dojo/_base/connect',
        'dojo/_base/event',
        'esri/geometry/geometryEngine',
        'esri/toolbars/draw',
        'esri/symbols/SimpleMarkerSymbol', 'esri/symbols/SimpleLineSymbol', 'esri/symbols/SimpleFillSymbol',
        'esri/layers/GraphicsLayer'
    ], function (
        declare,
        connect,
        event,
        geoEngine,
        Draw,
        SimpleMarkerSymbol, SimpleLineSymbol, SimpleFillSymbol,
        GraphicsLayer
        ) {
        var on = false;
        return declare('test.measureTool', null, {
            drawTools: null,
            symbols: null,
            _toolOn: false,
            //计数
            _LabelCount: 0,
            //初始偏移量
            _x: 0,
            _y: 0,
            gLayer: null,
            message: '起点',
            _measureCount: 0,
            currentMeasure: null,
            measures: {},
            constructor: function (map) {
                if (!on) {
                    on = true
                    this._map = map;
                    if (map.loaded) {
                        this.init();
                    } else {
                        var handle = connect.connect(map, 'onLoad', this, function () {
                            connect.disconnect(handle);
                            handle = null;
                            this.init();
                        });
                    }
                } else {
                    console.error('量算工具已经生成，请勿反复生成！');
                }
            },
            showTip: function () {
                $(this._toolTip).css('display', 'block');
            },
            hiddenTip: function () {
                $(this._toolTip).css('display', 'none');
            },
            init: function () {
                var map = this._map;

                this.initSymbols();
                this.initDrawTool();
                this.initStyle();

                //创建div容器
                var mapContainerId = map.container.id;
                var id = 'map_measuretool';
                //创建GraphicsLayer作为图形容器
                this.gLayer = new GraphicsLayer(id + '_gLayer');
                map.addLayer(this.gLayer);

                var $mapContainer = $('#' + mapContainerId + '_container');
                var $root = $('<div></div>').attr('id', id).appendTo($mapContainer);
                this._root = $root[0];
                this._toolTip = $('<div class="ct-measure-tooltip">tooltip</div>').appendTo($root)[0];

                //注册事件
                this._panHandle = connect.connect(map, 'onPan', this, this._translate);
                this._panEndHandle = connect.connect(map, 'onPanEnd', this, this._translateReset);

                this._zoomStartHandle = connect.connect(map, 'onZoomStart', this, this.hidden);
                this._zoomEndHandle = connect.connect(map, 'onZoomEnd', this, this.reposition);

                this._mapClickHandle = connect.connect(map, 'onClick', this, this._mapClick);
                this._mapMouseMoveHandle = connect.connect(map, 'onMouseMove', this, this._onMouseMove);

            },
            initSymbols: function () {
                this.symbols = {
                    point: new SimpleMarkerSymbol(esri.symbol.SimpleMarkerSymbol.STYLE_CIRCLE, 6, new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([238, 59, 59]), 2), new dojo.Color([255, 255, 255])),
                    polyline: new SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([238, 59, 59]), 2),
                    polygon: new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new dojo.Color([238, 59, 59]), 2), new dojo.Color([0, 0, 0, 0.3]))
                };
            },
            initDrawTool: function () {
                var map = this._map;
                var multiPointsTool = new Draw(map, { showTooltips: false });
                var comGeometryTool = new Draw(map, { showTooltips: false });

                multiPointsTool.markerSymbol = this.symbols.point;
                comGeometryTool.markerSymbol = this.symbols.point;
                comGeometryTool.fillSymbol = this.symbols.polygon;
                comGeometryTool.lineSymbol = this.symbols.polyline;

                this.drawTools = {
                    multiPointsTool: multiPointsTool,
                    comGeometryTool: comGeometryTool
                };

                this._drawCompleteHandle = connect.connect(comGeometryTool, 'onDrawComplete', this, this._drawComplete);
            },
            initStyle: function () {
                var style = document.createElement("style"),
str = "#map_measuretool { position: absolute; top: 0; left: 0; width: 0; height: 0; overflow: visible; z-index: 9; }"
         + ".ct-measure-tooltip { position: absolute; white-space: nowrap; background-color: rgba(0, 0, 0, 0.6); color: white; height: 20px; line-height: 20px; padding: 2px 10px; margin-top: -12px; margin-left: 10px; border-radius: 5px; font-size: 12px; display: none; }"
             + ".ct-measure-tooltip::after { content: ''; position: absolute; border: 5px solid transparent; border-right: 4px solid rgba(0, 0, 0, 0.6); top: 7px; left: -9px; }"
                + ".ct-measure-label { height: 20px; line-height: 20px; padding: 2px 10px; margin-top: -13px; margin-left: 8px; border-radius: 5px; background-color: rgba(255, 255, 255, 1); border: 1px solid rgb(238, 59, 59); color: #666; font-size: 12px; white-space: nowrap; }"
                + ".ct-measure-label::after { content: ''; position: absolute; border: 5px solid transparent; border-right: 4px solid rgb(238, 59, 59); top: 7px; left: -10px; }"
                + ".ct-measure-delete { cursor: pointer; margin-left: 5px; }";
                style.type = "text/css";
                style.innerHTML = str;
                document.getElementsByTagName("head")[0].appendChild(style);
            },
            start: function (type) {
                switch (type) {
                    case 'area':
                        type = 'polygon';
                        break;
                    case 'length':
                        type = 'polyline';
                        break;
                    default:
                        type = 'point';
                        break;
                }
                this._type = type;
                this._toolOn = true;
                this.showTip();
                this.message = '起点';

                if (type != 'point') {
                    this.drawTools.comGeometryTool.activate(type);
                    this.drawTools.multiPointsTool.activate('multipoint');
                } else {
                    this.drawTools.comGeometryTool.activate(type);
                }
                this.currentMeasure = {
                    graphic: null,
                    graphic2: null,
                    labels: [],
                    points: []
                };
            },
            stop: function () {
                this.hiddenTip();
                this._toolOn = false;
                this.drawTools.comGeometryTool.deactivate();
                this.drawTools.multiPointsTool.deactivate();
                this.message = '起点';
                var labels = this.currentMeasure.labels;
                for (var i = 0; i < labels.length; i++) {
                    labels[i].remove();
                }
                this.currentMeasure = null;
            },
            _onMouseMove: function (evt) {
                if (this._toolOn) {
                    var mapPoint = evt.mapPoint;
                    var x = evt.screenPoint.x - this._x;
                    var y = evt.screenPoint.y - this._y;

                    if (this._type != 'point') {
                        var pnts = this.currentMeasure.points.concat([mapPoint]);
                        var v = this.calculate(pnts);
                        if (!v) {
                            v = '继续添加';
                        }

                    } else {
                        var v = this.calculate([evt.mapPoint]);
                    }
                    $(this._toolTip).html(v).css({
                        top: y + 'px',
                        left: x + 'px'
                    });
                }
            },
            _drawComplete: function (evt) {
                var geo = evt.geometry;
                var symbol = geo.type == 'polyline' ? this.symbols.polyline : (geo.type == 'point' ? this.symbols.point : this.symbols.polygon);
                var graphic = new esri.Graphic(geo, symbol);
                this.gLayer.add(graphic);

                this.currentMeasure.graphic = graphic;
                var $lastLabel = this.currentMeasure.labels[this.currentMeasure.labels.length - 1];

                if (this._type != 'point') {
                    var pnts = this.currentMeasure.points;
                    var ps = [];
                    for (var i = 0; i < pnts.length; i++) {
                        ps.push([pnts[i].x, pnts[i].y]);
                    }
                    var multiPoint = new esri.geometry.Multipoint({
                        points: ps,
                        spatialReference: this._map.spatialReference
                    });
                    var gMultiPoint = new esri.Graphic(multiPoint, this.symbols.point);
                    this.gLayer.add(gMultiPoint);
                    this.currentMeasure.graphic2 = gMultiPoint;
                    $lastLabel.html('总：' + $lastLabel.html());
                }

                this.currentMeasure.id = 'm' + this._measureCount;
                this.measures['m' + this._measureCount] = this.currentMeasure;

                var btnClose = $('<span class="ct-measure-delete">×</span>').data('id', this.currentMeasure.id).appendTo($lastLabel)[0];
                connect.connect(btnClose, 'click', this, this._clearMeasure);

                this.message = '起点';
                this._measureCount++;
                this.currentMeasure = {
                    graphic: null,
                    graphic2: null,
                    labels: [],
                    points: []
                };
            },
            _clearMeasure: function (evt) {
                var $target = $(evt.target);
                var id = $target.data('id');
                this.clear(id);
                event.stop(evt);
            },
            clear: function (id) {
                var m = this.measures[id];
                if (m) {
                    var gLayer = this.gLayer;
                    gLayer.remove(m.graphic);
                    gLayer.remove(m.graphic2);
                    for (var i = 0; i < m.labels.length; i++) {
                        m.labels[i].remove();
                    }
                    m.labels = null;
                    m.points = null;
                    m.graphic = null;
                    m.graphic2 = null;
                    delete this.measures[id];
                }
            },
            _translate: function (ex, pnt) {
                var translate = 'translate(' + (this._x + pnt.x) + 'px,' + (this._y + pnt.y) + 'px)';
                $(this._root).css('transform', translate);
            },
            _translateReset: function (ex, pnt) {
                //更新偏移量
                var vd = this._map.__visibleDelta;
                this._x = vd.x;
                this._y = vd.y;
            },
            hidden: function () {
                $(this._root).css('display', 'none');
            },
            show: function () {
                $(this._root).css('display', 'block');
            },
            //缩放时要素位置重新放置
            reposition: function () {
                var map = this._map;
                var extent = map.extent;
                var height = map.height;
                var width = map.width;
                var $doms = $(this._root).find('>div[data-id]');
                var l = $doms.length;
                var vd = map.__visibleDelta;

                for (var i = 0; i < l; i++) {
                    var $dom = $($doms[i]);
                    var mapPoint = $dom.data('mapPoint');
                    var screenPoint = esri.geometry.toScreenPoint(extent, width, height, mapPoint);
                    $dom.css({
                        top: screenPoint.y - vd.y + 'px',
                        left: screenPoint.x - vd.x + 'px'
                    });
                }
                this.show();
            },
            _mapClick: function (evt) {
                if (this._toolOn) {
                    var vd = this._map.__visibleDelta;
                    this._LabelCount++;

                    this.currentMeasure.points.push(evt.mapPoint);
                    var v = this.calculate(this.currentMeasure.points);

                    var $dom = $('<div class="ct-measure-label" data-id="' + this._LabelCount + '"></div>').css({
                        position: 'absolute',
                        top: evt.y - vd.y + 'px',
                        left: evt.x - vd.x + 'px'
                    }).data({ 'mapPoint': evt.mapPoint }).html(v);
                    if (v) {
                        $dom.appendTo($(this._root));
                        this.currentMeasure.labels.push($dom);
                    }
                }
            },
            calculate: function (pnts) {
                var l = pnts.length;
                if (this._type == 'polygon') {
                    var geo = this._getPolygon(pnts);
                    switch (l) {
                        case 0:
                        case 2:
                            return '';
                        case 1:
                            return '起点';
                        default:
                            var v = geoEngine.planarArea(geo, 'square-meters');
                            v = Math.abs(v);
                            v = v > 1000000 ? (v / 1000000).toFixed(2) + '平方千米' : v.toFixed(2) + '平方米';
                            return v;
                    }
                } else if (this._type == 'polyline') {
                    var geo = this._getPolyline(pnts);

                    switch (l) {
                        case 0:
                            return '';
                        case 1:
                            return '起点';
                        default:
                            var v = geoEngine.planarLength(geo, 'meters');
                            v = v > 1000 ? (v / 1000).toFixed(2) + '千米' : v.toFixed(2) + '米';
                            return v;
                    }
                } else if (this._type == 'point' && l > 0) {
                    var x1 = pnts[0].x.toFixed(2);
                    var y1 = pnts[0].y.toFixed(2);
                    var v = '( x:' + x1 + ' , y:' + y1 + ' )';
                    return v;
                }
                return '';
            },
            _getPolygon: function (pnts) {
                var l = pnts.length;
                var coordinates = [];
                for (var i = 0; i < l; i++) {
                    coordinates.push([pnts[i].x, pnts[i].y]);
                }
                var geo = new esri.geometry.Polygon(coordinates);
                geo.spatialReference = this._map.spatialReference;
                return geo;
            },
            _getPolyline: function (pnts) {
                var l = pnts.length;
                var coordinates = [];
                for (var i = 0; i < l; i++) {
                    coordinates.push([pnts[i].x, pnts[i].y]);
                }
                var geo = new esri.geometry.Polyline(coordinates);
                geo.spatialReference = this._map.spatialReference;
                return geo;
            }
        })
    });