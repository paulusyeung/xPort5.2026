--
-- ER/Studio 8.0 SQL Code Generation
-- Company :      .
-- Project :      xPort5.dm1
-- Author :       paulus
--
-- Date Created : Wednesday, December 10, 2025 03:09:14
-- Target DBMS : PostgreSQL 8.0
-- Target Objects: Tables/ Indexes/ Constraints

-- 
-- TABLE: article 
--

CREATE TABLE article(
    articleid          uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    sku                varchar(16)       NOT NULL,
    articlecode        varchar(32),
    articlename        varchar(255),
    articlename_chs    varchar(255),
    articlename_cht    varchar(255),
    categoryid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    agegradingid       uuid,
    colorid            uuid,
    originid           uuid,
    remarks            varchar(255),
    colorpattern       varchar(255),
    barcode            varchar(16),
    unitcost           decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    currencyid         uuid,
    status             int4              DEFAULT ((0)) NOT NULL,
    createdon          timestamp         DEFAULT (now()) NOT NULL,
    createdby          uuid              NOT NULL,
    modifiedon         timestamp         DEFAULT (now()) NOT NULL,
    modifiedby         uuid              NOT NULL,
    retired            boolean           DEFAULT (false) NOT NULL,
    retiredon          timestamp,
    retiredby          uuid,
    CONSTRAINT pk_article PRIMARY KEY (articleid)
)
;



-- 
-- TABLE: articlecustomer 
--

CREATE TABLE articlecustomer(
    articlecustomerid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    articleid            uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    customerid           uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean           DEFAULT (false) NOT NULL,
    custref              varchar(32),
    currencyid           uuid,
    fclprice             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclprice             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unitprice            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    daterevised          timestamp,
    notes                varchar(255),
    createdon            timestamp         DEFAULT (now()) NOT NULL,
    createdby            uuid              NOT NULL,
    modifiedon           timestamp         DEFAULT (now()) NOT NULL,
    modifiedby           uuid              NOT NULL,
    retired              boolean           DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_articlecustomer PRIMARY KEY (articlecustomerid)
)
;



-- 
-- TABLE: articlekeypicture 
--

CREATE TABLE articlekeypicture(
    articlekeypictureid    uuid    NOT NULL,
    articleid              uuid    NOT NULL,
    resourcesid            uuid    NOT NULL,
    CONSTRAINT pk_articlekeypicture PRIMARY KEY (articlekeypictureid)
)
;



-- 
-- TABLE: articlepackage 
--

CREATE TABLE articlepackage(
    articlepackageid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    articleid           uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    packageid           uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec          boolean           DEFAULT (false) NOT NULL,
    uomid               uuid,
    innerbox            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    outerbox            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    cuft                decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizelength_in       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizewidth_in        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizeheight_in       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizelength_cm       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizewidth_cm        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizeheight_cm       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightgross_lb      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightnet_lb        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightgross_kg      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightnet_kg        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    containerqty        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    containersize       varchar(4),
    createdon           timestamp         DEFAULT (now()) NOT NULL,
    createdby           uuid              NOT NULL,
    modifiedon          timestamp         DEFAULT (now()) NOT NULL,
    modifiedby          uuid              NOT NULL,
    retired             boolean           DEFAULT (false) NOT NULL,
    retiredon           timestamp,
    retiredby           uuid,
    CONSTRAINT pk_articlepackage PRIMARY KEY (articlepackageid)
)
;



-- 
-- TABLE: articleprice 
--

CREATE TABLE articleprice(
    articlepriceid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    articleid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    sku               varchar(16),
    currencyid        uuid,
    defaultrec        boolean           DEFAULT (false) NOT NULL,
    fclprice          decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclprice          decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unitprice         decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    notes             varchar(255),
    createdon         timestamp         DEFAULT (now()) NOT NULL,
    createdby         uuid              NOT NULL,
    modifiedon        timestamp         DEFAULT (now()) NOT NULL,
    modifiedby        uuid              NOT NULL,
    retired           boolean           DEFAULT (false) NOT NULL,
    retiredon         timestamp,
    retiredby         uuid,
    CONSTRAINT pk_articleprice PRIMARY KEY (articlepriceid)
)
;



-- 
-- TABLE: articlesupplier 
--

CREATE TABLE articlesupplier(
    articlesupplierid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    articleid            uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    supplierid           uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean           DEFAULT (false) NOT NULL,
    suppref              varchar(32),
    currencyid           uuid,
    fclcost              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclcost              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unitcost             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    notes                varchar(255),
    createdon            timestamp         DEFAULT (now()) NOT NULL,
    createdby            uuid              NOT NULL,
    modifiedon           timestamp         DEFAULT (now()) NOT NULL,
    modifiedby           uuid              NOT NULL,
    retired              boolean           DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_articlesupplier PRIMARY KEY (articlesupplierid)
)
;



-- 
-- TABLE: customer 
--

CREATE TABLE customer(
    customerid          uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    customercode        varchar(10),
    acnumber            varchar(10),
    customername        varchar(128),
    customername_chs    varchar(128),
    customername_cht    varchar(128),
    regionid            uuid,
    termsid             uuid,
    currencyid          uuid,
    creditlimit         numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    profitmargin        int4              DEFAULT ((0)) NOT NULL,
    blacklisted         boolean           DEFAULT (false) NOT NULL,
    remarks             varchar(255),
    status              int4              DEFAULT ((0)) NOT NULL,
    createdon           timestamp         DEFAULT (now()) NOT NULL,
    createdby           uuid              NOT NULL,
    modifiedon          timestamp         DEFAULT (now()) NOT NULL,
    modifiedby          uuid              NOT NULL,
    retired             boolean           DEFAULT (false) NOT NULL,
    retiredon           timestamp,
    retiredby           uuid,
    CONSTRAINT pk_customer PRIMARY KEY (customerid)
)
;



-- 
-- TABLE: customeraddress 
--

CREATE TABLE customeraddress(
    customeraddressid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    customerid           uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    addressid            uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean         DEFAULT (false) NOT NULL,
    addrtext             varchar(512),
    addrismailing        boolean         DEFAULT (false) NOT NULL,
    phone1_label         uuid,
    phone1_text          varchar(32),
    phone2_label         uuid,
    phone2_text          varchar(32),
    phone3_label         uuid,
    phone3_text          varchar(32),
    phone4_label         uuid,
    phone4_text          varchar(32),
    phone5_label         uuid,
    phone5_text          varchar(32),
    notes                varchar(255),
    createdon            timestamp       DEFAULT (now()) NOT NULL,
    createdby            uuid            NOT NULL,
    modifiedon           timestamp       DEFAULT (now()) NOT NULL,
    modifiedby           uuid            NOT NULL,
    retired              boolean         DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_customeraddress PRIMARY KEY (customeraddressid)
)
;



-- 
-- TABLE: customercontact 
--

CREATE TABLE customercontact(
    customercontactid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    customerid           uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean         DEFAULT (false) NOT NULL,
    salutationid         uuid,
    fullname             varchar(64),
    firstname            varchar(64),
    lastname             varchar(64),
    jobtitleid           uuid,
    phone1_label         uuid,
    phone1_text          varchar(32),
    phone2_label         uuid,
    phone2_text          varchar(32),
    phone3_label         uuid,
    phone3_text          varchar(32),
    phone4_label         uuid,
    phone4_text          varchar(32),
    phone5_label         uuid,
    phone5_text          varchar(32),
    notes                varchar(255),
    createdon            timestamp       DEFAULT (now()) NOT NULL,
    createdby            uuid            NOT NULL,
    modifiedon           timestamp       DEFAULT (now()) NOT NULL,
    modifiedby           uuid            NOT NULL,
    retired              boolean         DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_customercontact PRIMARY KEY (customercontactid)
)
;



-- 
-- TABLE: orderin 
--

CREATE TABLE orderin(
    orderinid        uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    innumber         varchar(10),
    indate           timestamp,
    customerid       uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    yourorderno      varchar(32),
    yourref          varchar(32),
    carrier          varchar(32),
    paymentterms     uuid,
    pricingterms     uuid,
    loadingport      uuid,
    dischargeport    uuid,
    destination      uuid,
    originid         uuid,
    sendfrom         varchar(16),
    sendto           varchar(16),
    revision         int4            DEFAULT ((0)) NOT NULL,
    shipmentdate     timestamp,
    remarks          varchar(255),
    remarks2         varchar(255),
    remarks3         varchar(255),
    inuse            boolean         DEFAULT (false) NOT NULL,
    status           int4            DEFAULT ((0)) NOT NULL,
    createdon        timestamp       DEFAULT (now()) NOT NULL,
    createdby        uuid            NOT NULL,
    modifiedon       timestamp       DEFAULT (now()) NOT NULL,
    modifiedby       uuid            NOT NULL,
    accessedon       timestamp,
    accessedby       uuid,
    retired          boolean         DEFAULT (false) NOT NULL,
    retiredon        timestamp,
    retiredby        uuid,
    CONSTRAINT pk_orderin PRIMARY KEY (orderinid)
)
;



-- 
-- TABLE: orderincharges 
--

CREATE TABLE orderincharges(
    orderinchargeid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderinid          uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    chargeid           uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    amount             numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    description        varchar(255),
    CONSTRAINT pk_orderincharges PRIMARY KEY (orderinchargeid)
)
;



-- 
-- TABLE: orderinitems 
--

CREATE TABLE orderinitems(
    orderinitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderinid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    orderscitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    qty               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyin             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyout            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    shippingmark      varchar(255),
    CONSTRAINT pk_orderinitems PRIMARY KEY (orderinitemsid)
)
;



-- 
-- TABLE: orderinshipment 
--

CREATE TABLE orderinshipment(
    orderinshipmentid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderinitemsid       uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    shipmentid           int4              NOT NULL,
    qty                  decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderinshipment PRIMARY KEY (orderinshipmentid)
)
;



-- 
-- TABLE: orderpc 
--

CREATE TABLE orderpc(
    orderpcid        uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    pcnumber         varchar(10),
    pcdate           timestamp,
    supplierid       uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    yourref          varchar(32),
    carrier          varchar(32),
    paymentterms     uuid,
    pricingterms     uuid,
    loadingport      uuid,
    dischargeport    uuid,
    destination      uuid,
    originid         uuid,
    sendfrom         varchar(16),
    sendto           varchar(16),
    shipmentdate     timestamp,
    remarks          varchar(255),
    remarks2         varchar(255),
    remarks3         varchar(255),
    revision         int4            DEFAULT ((0)) NOT NULL,
    inuse            boolean         DEFAULT (false) NOT NULL,
    status           int4            DEFAULT ((0)) NOT NULL,
    createdon        timestamp       DEFAULT (now()) NOT NULL,
    createdby        uuid            NOT NULL,
    modifiedon       timestamp       DEFAULT (now()) NOT NULL,
    modifiedby       uuid            NOT NULL,
    accessedon       timestamp,
    accessedby       uuid,
    retired          boolean         DEFAULT (false) NOT NULL,
    retiredon        timestamp,
    retiredby        uuid,
    CONSTRAINT pk_orderpc PRIMARY KEY (orderpcid)
)
;



-- 
-- TABLE: orderpcitems 
--

CREATE TABLE orderpcitems(
    orderpcitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderpcid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    orderscitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    qty               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyin             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyout            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderpcitems PRIMARY KEY (orderpcitemsid)
)
;



-- 
-- TABLE: orderpk 
--

CREATE TABLE orderpk(
    orderpkid       uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    pknumber        varchar(10),
    customerid      uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    fclfactor       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclfactor       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unit            varchar(6),
    measurement     varchar(10),
    inputmask       int4              DEFAULT ((0)) NOT NULL,
    remarks         varchar(255),
    currencyid      uuid,
    exchangerate    decimal(12, 6)    DEFAULT ((0)) NOT NULL,
    paymentterms    varchar(6),
    repayment       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    inuse           boolean           DEFAULT (false) NOT NULL,
    status          int4              DEFAULT ((0)) NOT NULL,
    createddate     timestamp,
    createduser     varchar(16),
    accesseddate    timestamp,
    accesseduser    varchar(16),
    modifieddate    timestamp,
    modifieduser    varchar(16),
    sendfrom        varchar(16),
    sendto          varchar(16),
    revision        int4              DEFAULT ((0)) NOT NULL,
    shipmentdate    timestamp,
    CONSTRAINT pk_orderpk PRIMARY KEY (orderpkid)
)
;



-- 
-- TABLE: orderpkitems 
--

CREATE TABLE orderpkitems(
    orderpkitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderpkid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    articleid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    packageid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    sample            int4              DEFAULT ((0)) NOT NULL,
    custref           varchar(16),
    innerbox          decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    outerbox          decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    volumn            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unitcost          decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    fcost             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    margin            decimal(8, 4)     DEFAULT ((0)) NOT NULL,
    fcl               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lcl               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderpkitems PRIMARY KEY (orderpkitemsid)
)
;



-- 
-- TABLE: orderpl 
--

CREATE TABLE orderpl(
    orderplid      uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    plnumber       varchar(10),
    pldate         timestamp,
    customerid     uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid        uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    sendfrom       varchar(16),
    sendto         varchar(16),
    totalqty       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyin     decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyout    decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalamount    numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    remarks        varchar(255),
    revision       int4              DEFAULT ((0)) NOT NULL,
    inuse          boolean           DEFAULT (false) NOT NULL,
    status         int4              DEFAULT ((0)),
    createdon      timestamp         DEFAULT (now()) NOT NULL,
    createdby      uuid              NOT NULL,
    modifiedon     timestamp         DEFAULT (now()) NOT NULL,
    modifiedby     uuid              NOT NULL,
    accessedon     timestamp,
    accessedby     uuid,
    retired        boolean           DEFAULT (false) NOT NULL,
    retiredon      timestamp,
    retiredby      uuid,
    CONSTRAINT pk_orderpl PRIMARY KEY (orderplid)
)
;



-- 
-- TABLE: orderplitems 
--

CREATE TABLE orderplitems(
    orderplitemsid    uuid    DEFAULT (uuid_generate_v4()) NOT NULL,
    orderplid         uuid    DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    orderqtitemid     uuid    DEFAULT (uuid_generate_v4()) NOT NULL,
    CONSTRAINT pk_orderplitems PRIMARY KEY (orderplitemsid)
)
;



-- 
-- TABLE: orderqt 
--

CREATE TABLE orderqt(
    orderqtid       uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    qtnumber        varchar(10),
    qtdate          timestamp,
    customerid      uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    pricemethod     int4              DEFAULT ((0)) NOT NULL,
    fclfactor       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclfactor       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unit            varchar(6),
    measurement     varchar(10),
    sampleqty       decimal(8, 4)     DEFAULT ((0)) NOT NULL,
    inputmask       int4              DEFAULT ((0)) NOT NULL,
    currencyid      uuid,
    exchangerate    decimal(12, 6)    DEFAULT ((0)) NOT NULL,
    termsid         uuid,
    repayment       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sendfrom        varchar(16),
    sendto          varchar(16),
    totalqty        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyin      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyout     decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalamount     numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    remarks         varchar(255),
    remarks2        varchar(255),
    remarks3        varchar(255),
    revision        int4              DEFAULT ((0)) NOT NULL,
    inuse           boolean           DEFAULT (false) NOT NULL,
    status          int4              DEFAULT ((0)) NOT NULL,
    createdon       timestamp         DEFAULT (now()) NOT NULL,
    createdby       uuid              NOT NULL,
    modifiedon      timestamp         DEFAULT (now()) NOT NULL,
    modifiedby      uuid              NOT NULL,
    accessedon      timestamp,
    accessedby      uuid,
    retired         boolean           DEFAULT (false) NOT NULL,
    retiredon       timestamp,
    retiredby       uuid,
    CONSTRAINT pk_orderqt PRIMARY KEY (orderqtid)
)
;



-- 
-- TABLE: orderqtcustshipping 
--

CREATE TABLE orderqtcustshipping(
    orderqtcustshippingid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderqtitemid            uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    shippedon                timestamp,
    qtyordered               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyshipped               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    status                   int4              DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderqtcustshipping PRIMARY KEY (orderqtcustshippingid)
)
;



-- 
-- TABLE: orderqtitems 
--

CREATE TABLE orderqtitems(
    orderqtitemid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderqtid        uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber       int4,
    articleid        uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    packageid        uuid,
    supplierid       uuid,
    particular       varchar(128),
    custref          varchar(16),
    pricetype        int4,
    factorycost      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    margin           decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    fcl              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lcl              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sampleqty        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qty              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unit             varchar(6),
    amount           numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    carton           int4              DEFAULT ((0)) NOT NULL,
    glaccount        varchar(16),
    refdocno         varchar(10),
    shippingmark     varchar(255),
    qtyin            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyout           decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderqtitems PRIMARY KEY (orderqtitemid)
)
;



-- 
-- TABLE: orderqtpackage 
--

CREATE TABLE orderqtpackage(
    orderqtpackageid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderqtitemid       uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    unit                varchar(6),
    innerbox            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    outerbox            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    cuft                decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizelength_in       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizewidth_in        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizeheight_in       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizelength_cm       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizewidth_cm        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    sizeheight_cm       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightgross_lb      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightnet_lb        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightgross_kg      decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    weightnet_kg        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    containerqty        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    containersize       varchar(4),
    CONSTRAINT pk_orderqtpackage PRIMARY KEY (orderqtpackageid)
)
;



-- 
-- TABLE: orderqtsupplier 
--

CREATE TABLE orderqtsupplier(
    orderqtsupplierid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderqtitemid        uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    suppref              varchar(32),
    currencyid           uuid,
    fclcost              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    lclcost              decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unitcost             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderqtsupplier PRIMARY KEY (orderqtsupplierid)
)
;



-- 
-- TABLE: orderqtsuppshipping 
--

CREATE TABLE orderqtsuppshipping(
    orderqtsuppshippingid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderqtitemid            uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    dateshipped              timestamp,
    qtyordered               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyshipped               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    status                   int4              DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderqtsuppshipping PRIMARY KEY (orderqtsuppshippingid)
)
;



-- 
-- TABLE: ordersc 
--

CREATE TABLE ordersc(
    orderscid        uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    scnumber         varchar(10),
    scdate           timestamp,
    customerid       uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    yourorderno      varchar(32),
    yourref          varchar(32),
    carrier          varchar(32),
    paymentterms     uuid,
    pricingterms     uuid,
    loadingport      uuid,
    dischargeport    uuid,
    destination      uuid,
    originid         uuid,
    sendfrom         varchar(16),
    sendto           varchar(16),
    shipmentdate     timestamp,
    remarks          varchar(255),
    remarks2         varchar(255),
    remarks3         varchar(255),
    revision         int4            DEFAULT ((0)) NOT NULL,
    inuse            boolean         DEFAULT (false) NOT NULL,
    status           int4            DEFAULT ((0)) NOT NULL,
    createdon        timestamp       DEFAULT (now()) NOT NULL,
    createdby        uuid            NOT NULL,
    modifiedon       timestamp       DEFAULT (now()) NOT NULL,
    modifiedby       uuid            NOT NULL,
    accessedon       timestamp,
    accessedby       uuid,
    retired          boolean         DEFAULT (false) NOT NULL,
    retiredon        timestamp,
    retiredby        uuid,
    CONSTRAINT pk_ordersc PRIMARY KEY (orderscid)
)
;



-- 
-- TABLE: orderscitems 
--

CREATE TABLE orderscitems(
    orderscitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderscid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    orderqtitemid     uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    qtyordered        decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyin             decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    qtyout            decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    CONSTRAINT pk_orderscitems PRIMARY KEY (orderscitemsid)
)
;



-- 
-- TABLE: ordersp 
--

CREATE TABLE ordersp(
    orderspid      uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    spnumber       varchar(10),
    spdate         timestamp,
    customerid     uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid        uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    sendfrom       varchar(16),
    sendto         varchar(16),
    totalqty       decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyin     decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalqtyout    decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    totalamount    numeric(19, 4)    DEFAULT ((0)) NOT NULL,
    remarks        varchar(255),
    revision       int4              DEFAULT ((0)) NOT NULL,
    inuse          boolean           DEFAULT (false) NOT NULL,
    status         int4              DEFAULT ((0)) NOT NULL,
    createdon      timestamp         DEFAULT (now()) NOT NULL,
    createdby      uuid              NOT NULL,
    modifiedon     timestamp         DEFAULT (now()) NOT NULL,
    modifiedby     uuid              NOT NULL,
    accessedon     timestamp,
    accessedby     uuid,
    retired        boolean           DEFAULT (false) NOT NULL,
    retiredon      timestamp,
    retiredby      uuid,
    CONSTRAINT pk_ordersp PRIMARY KEY (orderspid)
)
;



-- 
-- TABLE: orderspitems 
--

CREATE TABLE orderspitems(
    orderspitemsid    uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    orderspid         uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    linenumber        int4,
    orderqtitemid     uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    qty               decimal(12, 4)    DEFAULT ((0)) NOT NULL,
    unit              varchar(6),
    CONSTRAINT pk_orderspitems PRIMARY KEY (orderspitemsid)
)
;



-- 
-- TABLE: resources 
--

CREATE TABLE resources(
    resourcesid         uuid            NOT NULL,
    keyword             varchar(64),
    contenttype         int4            NOT NULL,
    originalfilename    varchar(255),
    saveasfilename      varchar(255),
    saveasfileid        varchar(255),
    createdon           timestamp       NOT NULL,
    createdby           uuid            NOT NULL,
    modifiedon          timestamp       NOT NULL,
    modifiedby          uuid            NOT NULL,
    CONSTRAINT pk_resources PRIMARY KEY (resourcesid)
)
;



-- 
-- TABLE: staff 
--

CREATE TABLE staff(
    staffid       uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    divisionid    uuid,
    groupid       uuid,
    staffcode     varchar(10),
    fullname      varchar(64),
    firstname     varchar(64),
    lastname      varchar(64),
    alias         varchar(64),
    login         varchar(64),
    password      varchar(32),
    remarks       varchar(255),
    status        int4            DEFAULT ((0)) NOT NULL,
    createdon     timestamp       DEFAULT (now()) NOT NULL,
    createdby     uuid            NOT NULL,
    modifiedon    timestamp       DEFAULT (now()) NOT NULL,
    modifiedby    uuid            NOT NULL,
    retired       boolean         DEFAULT (false) NOT NULL,
    retiredon     timestamp,
    retiredby     uuid,
    CONSTRAINT pk_staff PRIMARY KEY (staffid)
)
;



-- 
-- TABLE: staffaddress 
--

CREATE TABLE staffaddress(
    staffaddressid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    staffid           uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    addressid         uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec        boolean         DEFAULT (false) NOT NULL,
    addrtext          varchar(512),
    addrismailing     boolean         DEFAULT (false) NOT NULL,
    phone1_label      uuid,
    phone1_text       varchar(32),
    phone2_label      uuid,
    phone2_text       varchar(32),
    phone3_label      uuid,
    phone3_text       varchar(32),
    phone4_label      uuid,
    phone4_text       varchar(32),
    phone5_label      uuid,
    phone5_text       varchar(32),
    notes             varchar(255),
    createdon         timestamp       DEFAULT (now()) NOT NULL,
    createdby         uuid            NOT NULL,
    modifiedon        timestamp       DEFAULT (now()) NOT NULL,
    modifiedby        uuid            NOT NULL,
    retired           boolean         DEFAULT (false) NOT NULL,
    retiredon         timestamp,
    retiredby         uuid,
    CONSTRAINT pk_staffaddress PRIMARY KEY (staffaddressid)
)
;



-- 
-- TABLE: supplier 
--

CREATE TABLE supplier(
    supplierid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    suppliercode        varchar(10),
    suppliername        varchar(255),
    suppliername_chs    varchar(255),
    suppliername_cht    varchar(255),
    acnumber            varchar(10),
    regionid            uuid,
    termsid             uuid,
    remarks             varchar(255),
    status              int4            DEFAULT ((0)) NOT NULL,
    createdon           timestamp       DEFAULT (now()) NOT NULL,
    createdby           uuid            NOT NULL,
    modifiedon          timestamp       DEFAULT (now()) NOT NULL,
    modifiedby          uuid            NOT NULL,
    retired             boolean         DEFAULT (false) NOT NULL,
    retiredon           timestamp,
    retiredby           uuid,
    CONSTRAINT pk_supplier PRIMARY KEY (supplierid)
)
;



-- 
-- TABLE: supplieraddress 
--

CREATE TABLE supplieraddress(
    supplieraddressid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    supplierid           uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean         DEFAULT (false) NOT NULL,
    addressid            uuid            NOT NULL,
    addrtext             varchar(512),
    addrismailing        boolean         DEFAULT (false) NOT NULL,
    phone1_label         uuid,
    phone1_text          varchar(32),
    phone2_label         uuid,
    phone2_text          varchar(32),
    phone3_label         uuid,
    phone3_text          varchar(32),
    phone4_label         uuid,
    phone4_text          varchar(32),
    phone5_label         uuid,
    phone5_text          varchar(32),
    notes                varchar(255),
    createdon            timestamp       DEFAULT (now()) NOT NULL,
    createdby            uuid            NOT NULL,
    modifiedon           timestamp       DEFAULT (now()) NOT NULL,
    modifiedby           uuid            NOT NULL,
    retired              boolean         DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_supplieraddress PRIMARY KEY (supplieraddressid)
)
;



-- 
-- TABLE: suppliercontact 
--

CREATE TABLE suppliercontact(
    suppliercontactid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    supplierid           uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    defaultrec           boolean         DEFAULT (false) NOT NULL,
    salutationid         uuid,
    fullname             varchar(64),
    firstname            varchar(64),
    lastname             varchar(64),
    jobtitleid           uuid,
    phone1_label         uuid,
    phone1_text          varchar(32),
    phone2_label         uuid,
    phone2_text          varchar(32),
    phone3_label         uuid,
    phone3_text          varchar(32),
    phone4_label         uuid,
    phone4_text          varchar(32),
    phone5_label         uuid,
    phone5_text          varchar(32),
    notes                varchar(255),
    createdon            timestamp       DEFAULT (now()) NOT NULL,
    createdby            uuid            NOT NULL,
    modifiedon           timestamp       DEFAULT (now()) NOT NULL,
    modifiedby           uuid            NOT NULL,
    retired              boolean         DEFAULT (false) NOT NULL,
    retiredon            timestamp,
    retiredby            uuid,
    CONSTRAINT pk_suppliercontact PRIMARY KEY (suppliercontactid)
)
;



-- 
-- TABLE: systeminfo 
--

CREATE TABLE systeminfo(
    systemid       uuid            NOT NULL,
    ownername      varchar(255),
    metadataxml    xml,
    CONSTRAINT pk_systeminfo PRIMARY KEY (systemid)
)
;



-- 
-- TABLE: t_agegrading 
--

CREATE TABLE t_agegrading(
    agegradingid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentagegrading      uuid,
    agegradingcode        varchar(10),
    agegradingname        varchar(64),
    agegradingname_chs    varchar(64),
    agegradingname_cht    varchar(64),
    CONSTRAINT pk_t_agegrading PRIMARY KEY (agegradingid)
)
;



-- 
-- TABLE: t_barcode 
--

CREATE TABLE t_barcode(
    barcodeid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    barcodecode        varchar(32),
    barcodename        varchar(255),
    barcodename_chs    varchar(255),
    barcodename_cht    varchar(255),
    barcodetype        varchar(32),
    CONSTRAINT pk_t_barcode PRIMARY KEY (barcodeid)
)
;



-- 
-- TABLE: t_category 
--

CREATE TABLE t_category(
    categoryid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    deptid              uuid,
    classid             uuid,
    categorycode        varchar(3),
    categoryname        varchar(64),
    categoryname_chs    varchar(64),
    categoryname_cht    varchar(64),
    costingmethod       int4           DEFAULT ((0)) NOT NULL,
    inventorymethod     int4           DEFAULT ((0)) NOT NULL,
    taxmethod           varchar(1),
    CONSTRAINT pk_t_category PRIMARY KEY (categoryid)
)
;



-- 
-- TABLE: t_charge 
--

CREATE TABLE t_charge(
    chargeid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentcharge      uuid,
    chargecode        varchar(6),
    chargename        varchar(64),
    chargename_chs    varchar(64),
    chargename_cht    varchar(64),
    accode            varchar(10),
    dccode            varchar(1),
    CONSTRAINT pk_t_charge PRIMARY KEY (chargeid)
)
;



-- 
-- TABLE: t_city 
--

CREATE TABLE t_city(
    cityid           uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    countryid        uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    provinceid       uuid           DEFAULT (uuid_generate_v4()),
    citycode         varchar(10),
    cityphonecode    varchar(4),
    cityname         varchar(64),
    cityname_chs     varchar(64),
    cityname_cht     varchar(64),
    CONSTRAINT pk_t_city PRIMARY KEY (cityid)
)
;



-- 
-- TABLE: t_class 
--

CREATE TABLE t_class(
    classid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    classcode        varchar(3),
    classname        varchar(64),
    classname_chs    varchar(64),
    classname_cht    varchar(64),
    CONSTRAINT pk_t_class PRIMARY KEY (classid)
)
;



-- 
-- TABLE: t_color 
--

CREATE TABLE t_color(
    colorid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentcolor      uuid,
    colorcode        varchar(10),
    colorname        varchar(64),
    colorname_chs    varchar(64),
    colorname_cht    varchar(64),
    CONSTRAINT pk_t_color PRIMARY KEY (colorid)
)
;



-- 
-- TABLE: t_country 
--

CREATE TABLE t_country(
    countryid           uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    countrycode         varchar(3),
    countryname         varchar(64)    NOT NULL,
    countryname_chs     varchar(64),
    countryname_cht     varchar(64),
    countryphonecode    varchar(4),
    CONSTRAINT pk_t_country PRIMARY KEY (countryid)
)
;



-- 
-- TABLE: t_currency 
--

CREATE TABLE t_currency(
    currencyid          uuid              DEFAULT (uuid_generate_v4()) NOT NULL,
    currencycode        varchar(3),
    currencyname        varchar(64),
    currencyname_chs    varchar(64),
    currencyname_cht    varchar(64),
    foreigncny          decimal(12, 6)    DEFAULT ((0)) NOT NULL,
    localcny            decimal(12, 6)    DEFAULT ((0)) NOT NULL,
    xchgbase            int4              DEFAULT ((0)) NOT NULL,
    xchgrate            decimal(12, 6)    DEFAULT ((0)) NOT NULL,
    localcurrency       boolean           DEFAULT (false) NOT NULL,
    CONSTRAINT pk_t_currency PRIMARY KEY (currencyid)
)
;



-- 
-- TABLE: t_dept 
--

CREATE TABLE t_dept(
    deptid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    deptcode        varchar(3),
    deptname        varchar(64),
    deptname_chs    varchar(64),
    deptname_cht    varchar(64),
    CONSTRAINT pk_t_dept PRIMARY KEY (deptid)
)
;



-- 
-- TABLE: t_division 
--

CREATE TABLE t_division(
    divisionid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentdivision      uuid,
    divisioncode        varchar(3),
    divisionname        varchar(64),
    divisionname_chs    varchar(64),
    divisionname_cht    varchar(64),
    CONSTRAINT pk_t_division PRIMARY KEY (divisionid)
)
;



-- 
-- TABLE: t_group 
--

CREATE TABLE t_group(
    groupid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentgroup      uuid,
    groupcode        varchar(3),
    groupname        varchar(64),
    groupname_chs    varchar(64),
    groupname_cht    varchar(64),
    CONSTRAINT pk_t_group PRIMARY KEY (groupid)
)
;



-- 
-- TABLE: t_origin 
--

CREATE TABLE t_origin(
    originid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentorigin      uuid,
    origincode        varchar(6),
    originname        varchar(64),
    originname_chs    varchar(64),
    originname_cht    varchar(64),
    CONSTRAINT pk_t_origin PRIMARY KEY (originid)
)
;



-- 
-- TABLE: t_package 
--

CREATE TABLE t_package(
    packageid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentpackage      uuid,
    packagecode        varchar(6),
    packagename        varchar(64),
    packagename_chs    varchar(64),
    packagename_cht    varchar(64),
    CONSTRAINT pk_t_package PRIMARY KEY (packageid)
)
;



-- 
-- TABLE: t_paymentterms 
--

CREATE TABLE t_paymentterms(
    termsid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentterms      uuid,
    termstype        varchar(1),
    termscode        varchar(6),
    termsname        varchar(64),
    termsname_chs    varchar(64),
    termsname_cht    varchar(64),
    creditdays       int4           DEFAULT ((0)) NOT NULL,
    monthlyac        boolean        DEFAULT (false) NOT NULL,
    CONSTRAINT pk_t_paymentterms PRIMARY KEY (termsid)
)
;



-- 
-- TABLE: t_port 
--

CREATE TABLE t_port(
    portid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentport      uuid,
    portcode        varchar(6),
    portname        varchar(64),
    portname_chs    varchar(64),
    portname_cht    varchar(64),
    CONSTRAINT pk_t_port PRIMARY KEY (portid)
)
;



-- 
-- TABLE: t_province 
--

CREATE TABLE t_province(
    provinceid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    provincecode        varchar(2),
    provincename        varchar(64),
    provincename_chs    varchar(64),
    provincename_cht    varchar(64),
    CONSTRAINT pk_t_province PRIMARY KEY (provinceid)
)
;



-- 
-- TABLE: t_region 
--

CREATE TABLE t_region(
    regionid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    parentregion      uuid,
    regioncode        varchar(6),
    regionname        varchar(64),
    regionname_chs    varchar(64),
    regionname_cht    varchar(64),
    CONSTRAINT pk_t_region PRIMARY KEY (regionid)
)
;



-- 
-- TABLE: t_remarks 
--

CREATE TABLE t_remarks(
    remarkid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    parentremark      uuid,
    remarkcode        varchar(6),
    remarkname        varchar(255),
    remarkname_chs    varchar(255),
    remarkname_cht    varchar(255),
    CONSTRAINT pk_t_remarks PRIMARY KEY (remarkid)
)
;



-- 
-- TABLE: t_shippingmark 
--

CREATE TABLE t_shippingmark(
    shippingmarkid          uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    parentshippingmark      uuid,
    shippingmarkcode        varchar(6),
    shippingmarkname        varchar(255),
    shippingmarkname_chs    varchar(255),
    shippingmarkname_cht    varchar(255),
    CONSTRAINT pk_t_shippingmark PRIMARY KEY (shippingmarkid)
)
;



-- 
-- TABLE: t_unitofmeasures 
--

CREATE TABLE t_unitofmeasures(
    uomid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    uomcode        varchar(6),
    uomname        varchar(64),
    uomname_chs    varchar(64),
    uomname_cht    varchar(64),
    CONSTRAINT pk_t_unitofmeasures PRIMARY KEY (uomid)
)
;



-- 
-- TABLE: userdisplaypreference 
--

CREATE TABLE userdisplaypreference(
    preferenceid          uuid           NOT NULL,
    userid                uuid           NOT NULL,
    preferenceobjectid    uuid,
    metadataxml           xml,
    CONSTRAINT pk_userdisplaypreference PRIMARY KEY (preferenceid)
)
;



-- 
-- TABLE: userprofile 
--

CREATE TABLE userprofile(
    userid           uuid           NOT NULL,
    usersid          uuid           NOT NULL,
    usertype         int4,
    loginname        varchar(64),
    loginpassword    varchar(32),
    alias            varchar(64),
    CONSTRAINT pk_userprofile PRIMARY KEY (userid)
)
;



-- 
-- TABLE: x_apppath 
--

CREATE TABLE x_apppath(
    apppathid    uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    program      varchar(255),
    picture      varchar(255),
    report       varchar(255),
    appdata      varchar(255),
    CONSTRAINT pk_x_apppath PRIMARY KEY (apppathid)
)
;



-- 
-- TABLE: x_counter 
--

CREATE TABLE x_counter(
    counterid          uuid    DEFAULT (uuid_generate_v4()) NOT NULL,
    nextsuppcode       int4,
    nextstaffcode      int4,
    nextcustcode       int4,
    nextskucode        int4,
    nextarticlecode    int4,
    nextqtnumber       int4,
    nextplnumber       int4,
    nextscnumber       int4,
    nextpcnumber       int4,
    nextinnumber       int4,
    nextpknumber       int4,
    nextspnumber       int4,
    CONSTRAINT pk_x_counter PRIMARY KEY (counterid)
)
;



-- 
-- TABLE: x_errorlog 
--

CREATE TABLE x_errorlog(
    errorlogid     uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    createddate    timestamp       NOT NULL,
    errorcode      varchar(10)     NOT NULL,
    errorlog       varchar(255),
    CONSTRAINT pk_x_errorlog PRIMARY KEY (errorlogid)
)
;



-- 
-- TABLE: x_eventlog 
--

CREATE TABLE x_eventlog(
    eventid        uuid            DEFAULT (uuid_generate_v4()) NOT NULL,
    createddate    timestamp       NOT NULL,
    eventcode      varchar(10)     NOT NULL,
    eventlog       varchar(255),
    CONSTRAINT pk_x_eventlog PRIMARY KEY (eventid)
)
;



-- 
-- TABLE: z_address 
--

CREATE TABLE z_address(
    addressid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    addresscode        varchar(3),
    addressname        varchar(64),
    addressname_chs    varchar(64),
    addressname_cht    varchar(64),
    CONSTRAINT pk_z_address PRIMARY KEY (addressid)
)
;



-- 
-- TABLE: z_email 
--

CREATE TABLE z_email(
    emailid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    emailcode        varchar(3),
    emailname        varchar(64),
    emailname_chs    varchar(64),
    emailname_cht    varchar(64),
    CONSTRAINT pk_z_email PRIMARY KEY (emailid)
)
;



-- 
-- TABLE: z_jobtitle 
--

CREATE TABLE z_jobtitle(
    jobtitleid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    jobtitlecode        varchar(3),
    jobtitlename        varchar(64),
    jobtitlename_chs    varchar(64),
    jobtitlename_cht    varchar(64),
    CONSTRAINT pk_z_jobtitle PRIMARY KEY (jobtitleid)
)
;



-- 
-- TABLE: z_phone 
--

CREATE TABLE z_phone(
    phoneid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    phonecode        varchar(3),
    phonename        varchar(64)    NOT NULL,
    phonename_chs    varchar(64),
    phonename_cht    varchar(64),
    CONSTRAINT pk_z_phone PRIMARY KEY (phoneid)
)
;



-- 
-- TABLE: z_salutation 
--

CREATE TABLE z_salutation(
    salutationid          uuid           DEFAULT (uuid_generate_v4()) NOT NULL,
    salutationcode        varchar(3),
    salutationname        varchar(64),
    salutationname_chs    varchar(64),
    salutationname_cht    varchar(64),
    CONSTRAINT pk_z_salutation PRIMARY KEY (salutationid)
)
;



-- 
-- TABLE: article 
--

ALTER TABLE article ADD CONSTRAINT fk_t_agegrading_article 
    FOREIGN KEY (agegradingid)
    REFERENCES t_agegrading(agegradingid)
;

ALTER TABLE article ADD CONSTRAINT fk_t_category_article 
    FOREIGN KEY (categoryid)
    REFERENCES t_category(categoryid)
;

ALTER TABLE article ADD CONSTRAINT fk_t_color_article 
    FOREIGN KEY (colorid)
    REFERENCES t_color(colorid)
;

ALTER TABLE article ADD CONSTRAINT fk_t_currency_article 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;

ALTER TABLE article ADD CONSTRAINT fk_t_origin_article 
    FOREIGN KEY (originid)
    REFERENCES t_origin(originid)
;


-- 
-- TABLE: articlecustomer 
--

ALTER TABLE articlecustomer ADD CONSTRAINT fk_article_articlecustomer 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE articlecustomer ADD CONSTRAINT fk_customer_articlecustomer 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE articlecustomer ADD CONSTRAINT fk_t_currency_articlecustomer 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;


-- 
-- TABLE: articlekeypicture 
--

ALTER TABLE articlekeypicture ADD CONSTRAINT fk_article_articlekeypicture 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE articlekeypicture ADD CONSTRAINT fk_resources_articlekeypicture 
    FOREIGN KEY (resourcesid)
    REFERENCES resources(resourcesid)
;


-- 
-- TABLE: articlepackage 
--

ALTER TABLE articlepackage ADD CONSTRAINT fk_article_articlepackage 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE articlepackage ADD CONSTRAINT fk_t_package_articlepackage 
    FOREIGN KEY (packageid)
    REFERENCES t_package(packageid)
;

ALTER TABLE articlepackage ADD CONSTRAINT fk_t_unitofmeasures_articlepackage 
    FOREIGN KEY (uomid)
    REFERENCES t_unitofmeasures(uomid)
;


-- 
-- TABLE: articleprice 
--

ALTER TABLE articleprice ADD CONSTRAINT fk_article_articleprice 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE articleprice ADD CONSTRAINT fk_t_currency_articleprice 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;


-- 
-- TABLE: articlesupplier 
--

ALTER TABLE articlesupplier ADD CONSTRAINT fk_article_articlesupplier 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE articlesupplier ADD CONSTRAINT fk_supplier_articlesupplier 
    FOREIGN KEY (supplierid)
    REFERENCES supplier(supplierid)
;

ALTER TABLE articlesupplier ADD CONSTRAINT fk_t_currency_articlesupplier 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;


-- 
-- TABLE: customer 
--

ALTER TABLE customer ADD CONSTRAINT fk_t_currency_customer 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;

ALTER TABLE customer ADD CONSTRAINT fk_t_paymentterms_customer 
    FOREIGN KEY (termsid)
    REFERENCES t_paymentterms(termsid)
;

ALTER TABLE customer ADD CONSTRAINT fk_t_region_customer 
    FOREIGN KEY (regionid)
    REFERENCES t_region(regionid)
;


-- 
-- TABLE: customeraddress 
--

ALTER TABLE customeraddress ADD CONSTRAINT fk_customer_customeraddress 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE customeraddress ADD CONSTRAINT fk_z_address_customeraddress 
    FOREIGN KEY (addressid)
    REFERENCES z_address(addressid)
;


-- 
-- TABLE: customercontact 
--

ALTER TABLE customercontact ADD CONSTRAINT fk_customer_customercontact 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE customercontact ADD CONSTRAINT fk_z_jobtitle_customercontact 
    FOREIGN KEY (jobtitleid)
    REFERENCES z_jobtitle(jobtitleid)
;

ALTER TABLE customercontact ADD CONSTRAINT fk_z_salutation_customercontact 
    FOREIGN KEY (salutationid)
    REFERENCES z_salutation(salutationid)
;


-- 
-- TABLE: orderin 
--

ALTER TABLE orderin ADD CONSTRAINT fk_customer_orderin 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE orderin ADD CONSTRAINT fk_staff_orderin 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE orderin ADD CONSTRAINT fk_t_origin_orderin 
    FOREIGN KEY (originid)
    REFERENCES t_origin(originid)
;


-- 
-- TABLE: orderincharges 
--

ALTER TABLE orderincharges ADD CONSTRAINT fk_orderin_orderincharges 
    FOREIGN KEY (orderinid)
    REFERENCES orderin(orderinid)
;

ALTER TABLE orderincharges ADD CONSTRAINT fk_t_charge_orderincharges 
    FOREIGN KEY (chargeid)
    REFERENCES t_charge(chargeid)
;


-- 
-- TABLE: orderinitems 
--

ALTER TABLE orderinitems ADD CONSTRAINT fk_orderin_orderinitems 
    FOREIGN KEY (orderinid)
    REFERENCES orderin(orderinid)
;

ALTER TABLE orderinitems ADD CONSTRAINT fk_orderscitems_orderinitems 
    FOREIGN KEY (orderscitemsid)
    REFERENCES orderscitems(orderscitemsid)
;


-- 
-- TABLE: orderinshipment 
--

ALTER TABLE orderinshipment ADD CONSTRAINT fk_orderinitems_orderinshipment 
    FOREIGN KEY (orderinitemsid)
    REFERENCES orderinitems(orderinitemsid)
;


-- 
-- TABLE: orderpc 
--

ALTER TABLE orderpc ADD CONSTRAINT fk_staff_orderpc 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE orderpc ADD CONSTRAINT fk_supplier_orderpc 
    FOREIGN KEY (supplierid)
    REFERENCES supplier(supplierid)
;

ALTER TABLE orderpc ADD CONSTRAINT fk_t_origin_orderpc 
    FOREIGN KEY (originid)
    REFERENCES t_origin(originid)
;


-- 
-- TABLE: orderpcitems 
--

ALTER TABLE orderpcitems ADD CONSTRAINT fk_orderpc_orderpcitems 
    FOREIGN KEY (orderpcid)
    REFERENCES orderpc(orderpcid)
;

ALTER TABLE orderpcitems ADD CONSTRAINT fk_orderscitems_orderpcitems 
    FOREIGN KEY (orderscitemsid)
    REFERENCES orderscitems(orderscitemsid)
;


-- 
-- TABLE: orderpk 
--

ALTER TABLE orderpk ADD CONSTRAINT fk_customer_orderpk 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE orderpk ADD CONSTRAINT fk_staff_orderpk 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE orderpk ADD CONSTRAINT fk_t_currency_orderpk 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;


-- 
-- TABLE: orderpkitems 
--

ALTER TABLE orderpkitems ADD CONSTRAINT fk_article_orderpkitems 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE orderpkitems ADD CONSTRAINT fk_orderpk_orderpkitems 
    FOREIGN KEY (orderpkid)
    REFERENCES orderpk(orderpkid)
;

ALTER TABLE orderpkitems ADD CONSTRAINT fk_t_package_orderpkitems 
    FOREIGN KEY (packageid)
    REFERENCES t_package(packageid)
;


-- 
-- TABLE: orderpl 
--

ALTER TABLE orderpl ADD CONSTRAINT fk_customer_orderpl 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE orderpl ADD CONSTRAINT fk_staff_orderpl 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;


-- 
-- TABLE: orderplitems 
--

ALTER TABLE orderplitems ADD CONSTRAINT fk_orderpl_orderplitems 
    FOREIGN KEY (orderplid)
    REFERENCES orderpl(orderplid)
;

ALTER TABLE orderplitems ADD CONSTRAINT fk_orderqtitems_orderplitems 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;


-- 
-- TABLE: orderqt 
--

ALTER TABLE orderqt ADD CONSTRAINT fk_customer_orderqt 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE orderqt ADD CONSTRAINT fk_staff_orderqt 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE orderqt ADD CONSTRAINT fk_t_currency_orderqt 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;

ALTER TABLE orderqt ADD CONSTRAINT fk_t_paymentterms_orderqt 
    FOREIGN KEY (termsid)
    REFERENCES t_paymentterms(termsid)
;


-- 
-- TABLE: orderqtcustshipping 
--

ALTER TABLE orderqtcustshipping ADD CONSTRAINT fk_orderqtitems_orderqtcustshipping 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;


-- 
-- TABLE: orderqtitems 
--

ALTER TABLE orderqtitems ADD CONSTRAINT fk_article_orderqtitems 
    FOREIGN KEY (articleid)
    REFERENCES article(articleid)
;

ALTER TABLE orderqtitems ADD CONSTRAINT fk_orderqt_orderqtitems 
    FOREIGN KEY (orderqtid)
    REFERENCES orderqt(orderqtid)
;

ALTER TABLE orderqtitems ADD CONSTRAINT fk_supplier_orderqtitems 
    FOREIGN KEY (supplierid)
    REFERENCES supplier(supplierid)
;

ALTER TABLE orderqtitems ADD CONSTRAINT fk_t_package_orderqtitems 
    FOREIGN KEY (packageid)
    REFERENCES t_package(packageid)
;


-- 
-- TABLE: orderqtpackage 
--

ALTER TABLE orderqtpackage ADD CONSTRAINT fk_orderqtitems_orderqtpackage 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;


-- 
-- TABLE: orderqtsupplier 
--

ALTER TABLE orderqtsupplier ADD CONSTRAINT fk_orderqtitems_orderqtsupplier 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;

ALTER TABLE orderqtsupplier ADD CONSTRAINT fk_t_currency_orderqtsupplier 
    FOREIGN KEY (currencyid)
    REFERENCES t_currency(currencyid)
;


-- 
-- TABLE: orderqtsuppshipping 
--

ALTER TABLE orderqtsuppshipping ADD CONSTRAINT fk_orderqtitems_orderqtsuppshipping 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;


-- 
-- TABLE: ordersc 
--

ALTER TABLE ordersc ADD CONSTRAINT fk_customer_ordersc 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE ordersc ADD CONSTRAINT fk_staff_ordersc 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE ordersc ADD CONSTRAINT fk_t_origin_ordersc 
    FOREIGN KEY (originid)
    REFERENCES t_origin(originid)
;


-- 
-- TABLE: orderscitems 
--

ALTER TABLE orderscitems ADD CONSTRAINT fk_orderqtitems_orderscitems 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;

ALTER TABLE orderscitems ADD CONSTRAINT fk_ordersc_orderscitems 
    FOREIGN KEY (orderscid)
    REFERENCES ordersc(orderscid)
;


-- 
-- TABLE: ordersp 
--

ALTER TABLE ordersp ADD CONSTRAINT fk_customer_ordersp 
    FOREIGN KEY (customerid)
    REFERENCES customer(customerid)
;

ALTER TABLE ordersp ADD CONSTRAINT fk_staff_ordersp 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;


-- 
-- TABLE: orderspitems 
--

ALTER TABLE orderspitems ADD CONSTRAINT fk_orderqtitems_orderspitems 
    FOREIGN KEY (orderqtitemid)
    REFERENCES orderqtitems(orderqtitemid)
;

ALTER TABLE orderspitems ADD CONSTRAINT fk_ordersp_orderspitems 
    FOREIGN KEY (orderspid)
    REFERENCES ordersp(orderspid)
;


-- 
-- TABLE: staff 
--

ALTER TABLE staff ADD CONSTRAINT fk_t_division_staff 
    FOREIGN KEY (divisionid)
    REFERENCES t_division(divisionid)
;

ALTER TABLE staff ADD CONSTRAINT fk_t_group_staff 
    FOREIGN KEY (groupid)
    REFERENCES t_group(groupid)
;


-- 
-- TABLE: staffaddress 
--

ALTER TABLE staffaddress ADD CONSTRAINT fk_staff_staffaddress 
    FOREIGN KEY (staffid)
    REFERENCES staff(staffid)
;

ALTER TABLE staffaddress ADD CONSTRAINT fk_z_address_staffaddress 
    FOREIGN KEY (addressid)
    REFERENCES z_address(addressid)
;


-- 
-- TABLE: supplier 
--

ALTER TABLE supplier ADD CONSTRAINT fk_t_paymentterms_supplier 
    FOREIGN KEY (termsid)
    REFERENCES t_paymentterms(termsid)
;

ALTER TABLE supplier ADD CONSTRAINT fk_t_region_supplier 
    FOREIGN KEY (regionid)
    REFERENCES t_region(regionid)
;


-- 
-- TABLE: supplieraddress 
--

ALTER TABLE supplieraddress ADD CONSTRAINT fk_supplier_supplieraddress 
    FOREIGN KEY (supplierid)
    REFERENCES supplier(supplierid)
;

ALTER TABLE supplieraddress ADD CONSTRAINT fk_z_address_supplieraddress 
    FOREIGN KEY (addressid)
    REFERENCES z_address(addressid)
;


-- 
-- TABLE: suppliercontact 
--

ALTER TABLE suppliercontact ADD CONSTRAINT fk_supplier_suppliercontact 
    FOREIGN KEY (supplierid)
    REFERENCES supplier(supplierid)
;

ALTER TABLE suppliercontact ADD CONSTRAINT fk_z_jobtitle_suppliercontact 
    FOREIGN KEY (jobtitleid)
    REFERENCES z_jobtitle(jobtitleid)
;

ALTER TABLE suppliercontact ADD CONSTRAINT fk_z_salutation_suppliercontact 
    FOREIGN KEY (salutationid)
    REFERENCES z_salutation(salutationid)
;


-- 
-- TABLE: t_agegrading 
--

ALTER TABLE t_agegrading ADD CONSTRAINT fk_t_agegrading_t_agegrading 
    FOREIGN KEY (parentagegrading)
    REFERENCES t_agegrading(agegradingid)
;


-- 
-- TABLE: t_category 
--

ALTER TABLE t_category ADD CONSTRAINT fk_t_class_t_category 
    FOREIGN KEY (classid)
    REFERENCES t_class(classid)
;

ALTER TABLE t_category ADD CONSTRAINT fk_t_dept_t_category 
    FOREIGN KEY (deptid)
    REFERENCES t_dept(deptid)
;


-- 
-- TABLE: t_charge 
--

ALTER TABLE t_charge ADD CONSTRAINT fk_t_charge_t_charge 
    FOREIGN KEY (parentcharge)
    REFERENCES t_charge(chargeid)
;


-- 
-- TABLE: t_city 
--

ALTER TABLE t_city ADD CONSTRAINT fk_t_country_t_city 
    FOREIGN KEY (countryid)
    REFERENCES t_country(countryid)
;

ALTER TABLE t_city ADD CONSTRAINT fk_t_province_t_city 
    FOREIGN KEY (provinceid)
    REFERENCES t_province(provinceid)
;


-- 
-- TABLE: t_color 
--

ALTER TABLE t_color ADD CONSTRAINT fk_t_color_t_color 
    FOREIGN KEY (parentcolor)
    REFERENCES t_color(colorid)
;


-- 
-- TABLE: t_division 
--

ALTER TABLE t_division ADD CONSTRAINT fk_t_division_t_division 
    FOREIGN KEY (parentdivision)
    REFERENCES t_division(divisionid)
;


-- 
-- TABLE: t_group 
--

ALTER TABLE t_group ADD CONSTRAINT fk_t_group_t_group 
    FOREIGN KEY (parentgroup)
    REFERENCES t_group(groupid)
;


-- 
-- TABLE: t_origin 
--

ALTER TABLE t_origin ADD CONSTRAINT fk_t_origin_t_origin 
    FOREIGN KEY (parentorigin)
    REFERENCES t_origin(originid)
;


-- 
-- TABLE: t_package 
--

ALTER TABLE t_package ADD CONSTRAINT fk_t_package_t_package 
    FOREIGN KEY (parentpackage)
    REFERENCES t_package(packageid)
;


-- 
-- TABLE: t_paymentterms 
--

ALTER TABLE t_paymentterms ADD CONSTRAINT fk_t_paymentterms_t_paymentterms 
    FOREIGN KEY (parentterms)
    REFERENCES t_paymentterms(termsid)
;


-- 
-- TABLE: t_port 
--

ALTER TABLE t_port ADD CONSTRAINT fk_t_port_t_port 
    FOREIGN KEY (parentport)
    REFERENCES t_port(portid)
;


-- 
-- TABLE: t_region 
--

ALTER TABLE t_region ADD CONSTRAINT fk_t_region_t_region 
    FOREIGN KEY (parentregion)
    REFERENCES t_region(regionid)
;


-- 
-- TABLE: t_remarks 
--

ALTER TABLE t_remarks ADD CONSTRAINT fk_t_remarks_t_remarks 
    FOREIGN KEY (parentremark)
    REFERENCES t_remarks(remarkid)
;


-- 
-- TABLE: t_shippingmark 
--

ALTER TABLE t_shippingmark ADD CONSTRAINT fk_t_shippingmark_t_shippingmark 
    FOREIGN KEY (parentshippingmark)
    REFERENCES t_shippingmark(shippingmarkid)
;


-- 
-- TABLE: userdisplaypreference 
--

ALTER TABLE userdisplaypreference ADD CONSTRAINT fk_userprofile_userdisplaypreference 
    FOREIGN KEY (userid)
    REFERENCES userprofile(userid)
;


