import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceListComponent } from './partner-advance-list.component';

describe('PartnerAdvanceListComponent', () => {
  let component: PartnerAdvanceListComponent;
  let fixture: ComponentFixture<PartnerAdvanceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
