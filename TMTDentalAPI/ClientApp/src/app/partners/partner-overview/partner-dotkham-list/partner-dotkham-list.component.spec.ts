import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerDotkhamListComponent } from './partner-dotkham-list.component';

describe('PartnerDotkhamListComponent', () => {
  let component: PartnerDotkhamListComponent;
  let fixture: ComponentFixture<PartnerDotkhamListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerDotkhamListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerDotkhamListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
