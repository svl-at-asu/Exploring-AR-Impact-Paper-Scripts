// JavaScript source code
import * as d3 from "https://cdn.jsdelivr.net/npm/d3@7/+esm";

// Define the program constants.
const numTeams = 6;
const numTrialsPerTeam = 12;
const thresholdAngle = 43.3;

// Define the color scheme and legend data.
const legendData = [
	{ text: "P1 Angle", color: "steelblue"},
	{ text: "P2 Angle", color: "black" },
	{ text: "Distance", color: "grey" },
	{ text: "Angle Threshold", color: "red" }
];

// Set the dimensions of the canvas / graph
var margin = { top: 10, right: 20, bottom: 50, left: 50 },
    width = 800 - margin.left - margin.right,
	height = 470 - margin.top - margin.bottom;

// Define function for parsing datetimes.
var parseTime = d3.timeParse("%m/%d/%Y %H:%M:%S %p");

// Set the time scale for the x axis and linear scale for the y axis.
const xScale = d3.scaleTime().range([0, width]);
const yScale = d3.scaleLinear().range([height, 0]);

// Set the distance scale for the participant distance.
const distScale = d3.scaleLinear().range([height, 0]);

for (var teamNum = 1; teamNum <= numTeams; teamNum++)
{
	for (var trialNum = 1; trialNum <= numTrialsPerTeam; trialNum++)
	{
		drawChart(teamNum, trialNum);
    }
}

async function drawChart(teamNum, trialNum) {
	// append the svg obgect to the body of the page
	// appends a 'group' element to 'svg'
	// moves the 'group' element to the top left margin
	var svg = d3.select(".root").append("svg")
		.attr("width", width + margin.left + margin.right)
		.attr("height", height + margin.top + margin.bottom)
		.append("g")
		.attr("transform",
			"translate(" + margin.left + "," + margin.top + ")");

	const data = await d3.csv("data/Angles_Team" + teamNum + "_Trial" + trialNum + ".csv");
	//console.log(data);

	// Format the data (parsing the strings to their appropriate datatypes)
	data.forEach(function (d) {
		d.time = parseTime(d.time);
		//console.log(d);
		d.p1_angle = Number(d.p1_angle);
		d.p2_angle = Number(d.p2_angle);
		d.distance = Number(d.distance);
	});

	// Set the time scale range based on the data's time range.
	xScale.domain(d3.extent(data, function (d) { return d.time; }));
	yScale.domain([0, 180]);

	// Set distance scale as a constant 5 meters to represent the maximum of the space.
	distScale.domain([0, 5]);

	// Define the curves.
	const p1_line = d3.line()
		.x((d) => { return xScale(d.time); })
		.y((d) => { return yScale(d.p1_angle); })
	//.curve(d3.curveCardinal);
	const p2_line = d3.line()
		.x((d) => { return xScale(d.time); })
		.y((d) => { return yScale(d.p2_angle); })
	//.curve(d3.curveCardinal);

	const dist_line = d3.line()
		.x((d) => { return xScale(d.time); })
		.y((d) => { return distScale(d.distance); })
	//.curve(d3.curveCardinal);

	svg.append("path").datum(data)
		.attr("fill", "none")
		.attr("stroke", legendData[0].color )
		.attr("stroke-linejoin", "round")
		.attr("stroke-linecap", "round")
		.attr("stroke-width", 1.5)
		.attr("d", p1_line);
	svg.append("path").datum(data)
		.attr("fill", "none")
		.attr("stroke", legendData[1].color)
		.attr("stroke-linejoin", "round")
		.attr("stroke-linecap", "round")
		.attr("stroke-width", 1.5)
		.attr("d", p2_line);
	svg.append("path").datum(data)
		.attr("fill", "none")
		.attr("stroke", legendData[2].color)
		.attr("stroke-linejoin", "round")
		.attr("stroke-linecap", "round")
		.attr("stroke-width", 1.5)
		.style("stroke-dasharray", "3,3")
		.style("opacity", 0.3)
		.attr("d", dist_line);

	// Add the threshold line.
	svg.append("line")
		.attr("class", "y")
		.attr("stroke", legendData[3].color)
		.style("stroke-dasharray", "3,3")
		.style("opacity", 0.5)
		.attr("x1", 0)
		.attr("x2", width)
		.attr("y1", yScale(thresholdAngle))
		.attr("y2", yScale(thresholdAngle));

	var customXScale = d3.scaleLinear().range([0, width]).domain([0, 120])

	// Define the x Axis
	const xAxis = d3.axisBottom(customXScale)
	//.ticks(11, ".0s");

	// Add the X Axis
	svg.append("g")
		.attr("transform", "translate(0," + height + ")")
		.attr("font-family", "Lato")
		.call(xAxis);

	// Add the X Axis Label
	svg.append("text")
		.attr("font-family", "sans-serif")
		.attr("font-size", 14)
		.attr("font-weight", 700)
		.attr("text-anchor", "middle")
		.attr("x", width / 2)
		.attr("y", height + margin.bottom - 5)
		.text("Time");

	// Add the Y Axis
	svg.append("g")
		.call(d3.axisLeft(yScale));

	// Add the Y Axis Label
	svg.append("text")
		.attr("font-family", "sans-serif")
		.attr("font-size", 14)
		.attr("font-weight", 700)
		.attr("text-anchor", "middle")
		.attr("x", 0 - (height / 2))
		.attr("y", 10 - margin.left)
		.attr("transform", "rotate(-90)")
		.text("Angle");

	// Add the title
	svg.append("text")
		.attr("font-family", "sans-serif")
		.attr("font-size", 16)
		.attr("font-weight", 700)
		.attr("text-decoration", "underline")
		.attr("text-anchor", "middle")
		.attr("x", width / 2)
		.attr("y", 12 - margin.top)
		.text("Participant Angles: Team " + teamNum + " Trial " + trialNum);

	// Add the legend
	var legend = svg.append("g")
		.attr("transform", "translate(" + (width - 100) + "," + 25 + ")");

	var legendRect = legend.selectAll('g')
		.data(legendData);

	var legendRectE = legendRect.enter()
		.append("g")
		.attr("transform", function (d, i) {
			return 'translate(0, ' + (i * 20) + ')';
		});

	legendRectE
		.append('rect')
		.attr("width", 15)
		.attr("height", 15)
		.style("fill", function (d) { return d.color; });

	legendRectE
		.append("text")
		.attr("x", 20)
		.attr("y", 10)
		.attr("font-family", "sans-serif")
		.attr("font-size", 11)
		.text(function (d) { return d.text; });
}


