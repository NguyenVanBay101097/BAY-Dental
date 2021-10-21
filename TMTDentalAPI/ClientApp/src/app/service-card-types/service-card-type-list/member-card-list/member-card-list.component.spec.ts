import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberCardListComponent } from './member-card-list.component';

describe('MemberCardListComponent', () => {
  let component: MemberCardListComponent;
  let fixture: ComponentFixture<MemberCardListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemberCardListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberCardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
