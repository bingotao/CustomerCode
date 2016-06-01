; (function (w, undefined) {
    widgets = w.widgets || {};
    widgets.InitTab = InitTab;
    /*
    初始化Tab页
    */
    function InitTab($root, index, callBack) {
        var tbs = {
            $dom: null,
            fun: {},
            callBack: callBack,
            cls: {
                active: 'active',
                target: 'target',
                nav: 'ct-nav',
                navPanel: 'ct-nav-container-panel'
            }
        };

        tbs.fun.InitDom = InitDom;
        tbs.fun.SelectItem = SelectItem;

        function InitDom($root) {
            var $navLi = $root.find('li');
            var $navPanels = $root.find('.ct-nav-container-panel>div');
            tbs.$dom = {
                $root: $root,
                $navLi: $navLi,
                $navPanels: $navPanels
            };
            $navLi.click(function () {
                var $this = $(this);
                $navLi.removeClass(tbs.cls.active);
                $this.addClass(tbs.cls.active);
                var target = $this.data(tbs.cls.target);
                $navPanels.removeClass(tbs.cls.active).filter('[data-source=' + target + ']').addClass(tbs.cls.active);
                if (tbs.callBack && tbs.callBack.navItemClick) {
                    tbs.callBack.navItemClick($this);
                }
            });
        }

        function SelectItem(index) {
            $(tbs.$dom.$navLi[index]).click();
        }

        InitDom($root);
        tbs.fun.SelectItem(index ? index : 0);

        return tbs;
    }

    /*
    初始化时间轴
    */
    widgets.InitTimeline = InitTimeline;

    function InitTimeline($dom, timeArray, callback, bReverse) {
        var timeLine = {};
        var $line = $(' <div class="ct-timeline-line"></div>').appendTo($dom);
        bReverse ? timeArray.reverse() : null;

        var length = timeArray.length;
        for (var i = 0; i < length; i++) {
            var time = timeArray[i];
            var name = time.name;
            var $item = $('<div data-id="' + time.id + '" class="ct-timeline-item"><div class="ct-timeline-arrow"></div><div class="ct-timeline-circle"></div><div class="ct-timeline-item-content">' + name + '</div></div>');
            $item.appendTo($line).data('config', time);
        }

        timeLine.$dom = {
            $root: $dom,
            $line: $line,
            $items: $dom.find('.ct-timeline-item')
        };

        timeLine.fun = {
            select: select,
            getSelect: getSelect
        };

        timeLine.callback = callback ? callback : {};

        timeLine.$dom.$items.click(function () {
            var $this = $(this);
            if (!$this.hasClass('active'))
                select($this.data('id'));
        });

        function select(id) {
            timeLine.$dom.$items.removeClass('active');
            var $item = $(timeLine.$dom.$items.filter('[data-id=' + id + ']')).addClass('active');
            if (timeLine.callback && timeLine.callback.onSelect) {
                timeLine.callback.onSelect($item, $item.data('config'));
            }
        }

        function getSelect() {
            return timeLine.$dom.$items.filter('.active').data('config');
        }
        return timeLine;
    }

    widgets.InitLoading = InitLoading;
    widgets.DestroyLoading = DestroyLoading;

    /*
    $dom 需要添加loading的jQuery dom对象
    msg 提示信息 默认为空
    类型 三种loading样式 circle ball slider
    */
    function InitLoading($dom, msg, type) {
        type = type ? type : 'circle';
        msg = msg ? msg : '';
        var size = 'big';
        $('<div class="ct-loading ct-loading-' + size + '">' +
            '<div class="ct-loading-' + type + '"></div>' +
            '<div class="ct-loading-text">' + msg + '</div>' +
          '</div>').appendTo($dom);
    }

    function DestroyLoading($dom) {
        $dom.find('.ct-loading').remove();
    }
})(window);
