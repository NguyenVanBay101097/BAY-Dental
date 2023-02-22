import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerDotkhamDetailComponent } from './partner-dotkham-detail.component';

describe('PartnerDotkhamDetailComponent', () => {
  let component: PartnerDotkhamDetailComponent;
  let fixture: ComponentFixture<PartnerDotkhamDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerDotkhamDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerDotkhamDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
