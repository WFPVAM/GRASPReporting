var app = angular.module('angularjs-starter', []);
app.directive('jqdatepicker', function () {
    return {
        restrict: 'A', require: 'ngModel', link: function (scope, element, attrs, MainCtrl) {
            element.datepicker({
                dateFormat: 'yy-mm-dd', onSelect: function (date) {
                    var modName = this.getAttribute('ng-model').split(".");
                    var evalCmd = "scope." + modName[0] + "['" + modName[1] + "']  = '" + date + "'";
                    if (modName[0] == "currForm") scope.currForm[modName[1]] = date; else
                        eval(evalCmd);
                    scope.$apply();
                }
            });
        }
    };
});


app.controller('MainCtrl', function ($scope, $http) {
    $scope.master = {}; $scope.update = function (currForm) {
        $scope.master = angular.copy(currForm); sendJson(); unsaved = false;
    }; $scope.reset = function () {
        $scope.currForm = angular.copy($scope.master);
    };
    $scope.isUnchanged = function (currForm) {
        return angular.equals(currForm, $scope.master);
    };
    $scope.reset();
    $scope.currForm.date = "2014-10-23";
    $scope.currForm.des_version = "WEB";
    $scope.currForm.client_version = "WEB";
    $scope.options4 = [{ value: "Safe" }, { value: "Unsafe" }];
    $scope.options1 = [{ value: "Housing" }, { value: "Social Infrastructure" }, { value: "Economic Infrastructure" }];
    $scope.currForm.Q105bSps1ID = 0;
    $scope.currForm.Q106Sps2ID = 0;
    $scope.currForm.Q107bSps3ID = 0;
    $scope.currForm.Q108bSps4ID = 0;
    $scope.currForm.Q1010FamilyID = 0;
    $scope.currForm.Q1011 = 0;
    $scope.currForm.Q1012 = 0;
    $scope.currForm.Q1014 = [];
    $scope.addNewQ1014 = function () {
        $scope.currForm.Q1014.push({ Q1014a: '', Q1014bRCN: 2, Q104cFamilyID: 2, Q104dIDN: 2, Q1014e: 2 });
    };
    $scope.options6 = [{ value: "North شمال" }, { value: "Gaza غزة" }, { value: "Khan Younis خان يونس" }, { value: "Rafah رفح" }, { value: "Middle الوسطى" }];
    $scope.options7 = [{ value: "أم النصر (القرية البدوية) N10" }, { value: "بيت حانون N20" }, { value: "جباليا N30" }, { value: "بيت لاهيا N40" }];
    $scope.options2 = [{ value: "غزة G10" }, { value: "جحر الديك G20" }, { value: "المغراقة G30" }, { value: "مدينة الزهراء G40" }, { value: "الشجاعية G50" }];
    $scope.options9 = [{ value: "النصيرات M10" }, { value: "البريج M20" }, { value: "المغازي M30" }, { value: "المصدر M40" }, { value: "وادي السلقا M50" }, { value: "دير البلح M60" }, { value: "الزوايدة M70" }];
    $scope.options12 = [{ value: "خانيونس K10" }, { value: "القرارة K20" }, { value: "بني سهيلا K30" }, { value: "عبسان الجديدة (الصغيرة) K40" }, { value: "عبسان الكبيرة K50" }, { value: "خزاعة K60" }, { value: "الفخاري K70" }];
    $scope.options10 = [{ value: "رفح R10" }, { value: "النصر R20" }, { value: "الشوكة R30" }];
    $scope.options11 = [{ value: "Finished Concrete Building" }, { value: "Concrete Skeleton" }, { value: "Eternite Sheets" }, { value: "Iron Sheets" }, { value: "Combined" }, { value: "Others (Describe) general services…etc" }];
    $scope.options5 = [{ value: "No Damage" }, { value: "Repairable" }, { value: "Not Repairable" }]; $scope.options8 = [{ value: "Owned" }, { value: "Rented" }];
    $scope.currForm.Q3023mISC = [];
    $scope.addNewQ3023mISC = function () {
        $scope.currForm.Q3023mISC.push({ Q3023a: '', Q3023b: 2, Q3023C: 2 });
    };
    $scope.$watch("currForm.Q1005!='Yes'", function () {
        var key = "Q105a";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q1005!='Yes'", function () {
        var key = "Q105bSps1ID";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q1005!='Yes'", function () {
        var key = "Q106";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q106!='Yes'", function () {
        var key = "Q106a"; delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q106!='Yes'", function () {
        var key = "Q106Sps2ID";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q106!='Yes'", function () {
        var key = "Q107";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q107!='Yes'", function () {
        var key = "Q107aSps3Nm";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q107!='Yes'", function () {
        var key = "Q107bSps3ID";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q107!='Yes'", function () {
        var key = "Q108";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q108!='Yes'", function () {
        var key = "Q108asps4nm";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q108!='Yes'", function () {
        var key = "Q108bSps4ID";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q109!='Yes'", function () {
        var key = "SingleLine_1103355466";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q109!='Yes'", function () {
        var key = "Q1010FamilyID";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q109!='Yes'", function () {
        var key = "Q1010bAlt";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q109!='Yes'", function () {
        var key = "Q1011";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q109!='Yes'", function () {
        var key = "Q1012";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q1013!='Yes'", function () {
        $scope.currForm.Q1014 = [];
    });
    $scope.$watch("currForm.Q204Gov.value!='North شمال'", function () {
        var key = "Q204aNG";
        delete $scope.currForm[key];
    });
    $scope.$watch("currForm.Q204Gov.value!='Middle الوسطى'", function () { var key = "Q204c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q204Gov.value!='Khan Younis خان يونس'", function () { var key = "q2014d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q204Gov.value!='Rafah رفح'", function () { var key = "Q204E"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q301TypeBldg.value!='Others (Describe) general services…etc'", function () { var key = "q301typothers"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "SingleLine1522905722"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013k"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3130Dem!='Yes'", function () { var key = "Q3013l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "SingleLine_19165973"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3140!='Yes'", function () { var key = "Q3014f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "SingleLine128075482"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30150!='Yes'", function () { var key = "Q3015j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "MultiLine_396469556"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "SingleLine411265399"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016k"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016m"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016n"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016o"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016p"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016q"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016r"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016s"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016t"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016u"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016v"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3160!='Yes'", function () { var key = "Q3016w"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "SingleLine1958955194"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017k"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3170!='Yes'", function () { var key = "Q3017l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "SingleLine_444716286"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3180!='Yes'", function () { var key = "Q3018f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "SingleLine1573844446"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019K"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019m"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019n"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019o"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019p"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019q"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q3190!='Yes'", function () { var key = "Q3019r"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "SingleLine1580672949"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320Combreq!='Yes'", function () { var key = "Q3020j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "SingleLine_632179992"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "MultiLine_1038630227"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021k"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021m"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021n"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021o"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021p"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021q"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021r"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021s"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021t"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021u"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "MultiLine_1030327280"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021v"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021w"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021x"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021y"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021z"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021aa"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021ab"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021ac"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021ad"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320PluWorks!='Yes'", function () { var key = "Q3021ae"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "SingleLine_937977918"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022a"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022b"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022c"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022d"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022e"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022f"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022g"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022h"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022i"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022j"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022k"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022l"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022m"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022n"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "MultiLine595202615"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022o"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022p"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022q"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022r"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022s"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022t"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022u"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022v"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022w"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022x"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q320ELE!='Yes'", function () { var key = "Q3022y"; delete $scope.currForm[key]; }); $scope.$watch("currForm.Q30230!='Yes'", function () { $scope.currForm.Q3023mISC = []; });

    $http({
        method: 'GET',
        url: 'http://graspreporting.brainsen.com/formJSON.txt'
    }).success(function (data, status) {
        console.log('works!!!   ' + data);
        $scope.currForm = data;
    }).error(function (data, status) {
        // Some error occurred
        console.log(status);
        $scope.currForm = {
            "date": "2014-09-07",
            "des_version": "WEB",
            "client_version": "WEB",
            "Q105bSps1ID": 800503369,
            "enumerator": "كفاح جهاد صلاح حجي",
            "Latitude": "0",
            "Longitude": "0",
            "Q001SurvCode": 100,
            "Q002SrvName": "F001",
            "Q002AreaCode": "G3003F001",
            "Q004SecSit": { "value": "Safe" },
            "Q005TypInf": { "value": "Housing" },
            "Q006Comm": "0",
            "Q101": "G3003F001",
            "Q102a": "كفاح",
            "Q102b": "جهاد",
            "Q102c": "صلاح",
            "Q102d": "حجي",
            "Q103ID": 803520303,
            "Q104Mob": 598296821,
            "Q1005": "Yes",
            "Q105a": "وجيه حجي",
            "Q106": "No",
            "Q109": "No",
            "Q1013": "No",
            "Q204Gov": { "value": "Gaza غزة" },
            "Q204bG": { "value": "المغراقة G30" },
            "Q202Address": "بجوار شارع 16",
            "Q203": "0",
            "Q205DamDate": "2014-08-12",
            "Q301TypeBldg": { "value": "Finished Concrete Building" },
            "Q302NoFloors": 2,
            "Q303DEMFloors": "Ground + the second floor",
            "Q304UnitArea": 200,
            "Q305BoundWall": 0,
            "Q306a": "0",
            "Q306b": "0",
            "Q306c": "0",
            "Q307STRUCTsAFE": "Yes",
            "Q308": "No",
            "Q309typedamage": { "value": "Repairable" },
            "q3011a": "No",
            "Q3011SheltiHab": "Yes",
            "Q3012": { "value": "Owned" },
            "Q3013Occupied": "Yes",
            "Q3130Dem": "Yes",
            "Q3013a": 5,
            "Q3013c": 1,
            "Q3013e": 1,
            "Q3013i": 8,
            "Q3140": "Yes",
            "Q3014d": 5,
            "Q30150": "Yes",
            "Q3015f": 1,
            "Q3160": "Yes",
            "Q3016a": 20,
            "Q3016b": 20,
            "Q3016c": 20,
            "Q3016e": 250,
            "Q3016m": 10,
            "Q3016r": 6,
            "Q3170": "Yes",
            "Q3017a": 1.5,
            "Q3017k": 8,
            "Q3180": "Yes",
            "Q3018a": 4,
            "Q3018d": 2,
            "Q3190": "No",
            "Q320Combreq": "Yes",
            "Q3020a": 30,
            "Q3020b": 10,
            "Q320PluWorks": "No",
            "Q320ELE": "No",
            "Q30230": "Yes",
            "repairof": "0",
            "Recof": "0",
            "Remarks": "0",
            "Q3023mISC": [
            {
                "Q3023a": "قصارة إيطالية واجهات"
            , "Q3023b": 200
            , "Q3023C": 0
            }, {
                "Q3023a": "أعمال جبس أطراف جوانب السقف"
            , "Q3023b": 30
            , "Q3023C": 0
            }, {
                "Q3023a": "جبس زخرفة وسط الغرفة"
            , "Q3023b": 1
            , "Q3023C": 0
            }]
        }
    });
});