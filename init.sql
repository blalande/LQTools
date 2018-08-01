truncate table lignescore;
delete from scorecard;
delete from evenement;
delete from Centre;

insert into Centre values ('Global', NEWID());
insert into Centre values ('Maurepas', NEWID());
insert into Centre values ('Cergy', NEWID());

insert into ScoringSysteme values ('Standard',200,10,10,10,10,10,5,4,3,3);

insert into Evenement values ('Normal',1,'Planning Regulier',0,0,1);
insert into Evenement values ('Normal',2,'Planning Regulier',0,0,1);
insert into Evenement values ('Normal',3,'Planning Regulier',0,0,1);

select * from Centre